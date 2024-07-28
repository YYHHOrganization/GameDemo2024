//see this for ref: https://docs.unity3d.com/ScriptReference/Graphics.DrawMeshInstancedIndirect.html

using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Profiling;

// [ExecuteAlways]//如果有这个 脚本会在编辑器中运行，以便在编辑器中调整草地实例的数量和绘制距离。是为了在editor模式下调整参数
public class InstancedIndirectGrassRenderer : MonoBehaviour
{
    [Header("Settings")]
    public float drawDistance = 125;//this setting will affect performance a lot!
    public Material instanceMaterial;

    [Header("Internal")]
    public ComputeShader cullingComputeShader;

    [NonSerialized]
    //包含所有草地实例位置的列表。这个列表应该由用户使用C#代码进行更新
    public List<Vector3> allGrassPos = new List<Vector3>();//user should update this list using C#
    //=====================================================
    [HideInInspector]   
    public static InstancedIndirectGrassRenderer instance;// global ref to this script

    private int cellCountX = -1;
    private int cellCountZ = -1;
    private int dispatchCount = -1;

    //smaller the number, CPU needs more time, but GPU is faster
    private float cellSizeX = 10; //unity unit (m)
    private float cellSizeZ = 10; //unity unit (m)

    private int instanceCountCache = -1;
    private Mesh cachedGrassMesh;

    private ComputeBuffer allInstancesPosWSBuffer;
    private ComputeBuffer visibleInstancesOnlyPosWSIDBuffer;
    private ComputeBuffer argsBuffer;

    private List<Vector3>[] cellPosWSsList; //for binning: binning will put each posWS into correct cell
    private float minX, minZ, maxX, maxZ;
    private List<int> visibleCellIDList = new List<int>();
    private Plane[] cameraFrustumPlanes = new Plane[6];

    bool shouldBatchDispatch = true;
    //=====================================================

    private void OnEnable()
    {
        instance = this; // assign global ref using this script
    }
    public void ReSetGrass()
    {
        instanceCountCache = -1;
        couldRenderGrass = true;
    }
    private bool couldRenderGrass = false;
    public bool CouldRenderGrass
    {
        set
        {
            couldRenderGrass = value;
        }
    }
    
    void LateUpdate()
    {
        // if(instanceCountCache == -1)
        // {
        //     return;
        // }
        if (!couldRenderGrass) return;
        
        //更新所有草地实例的变换缓冲区。
        //如果草地实例的数量发生变化，或者草地实例的位置发生变化，那么就需要更新这个缓冲区。
        // recreate all buffers if needed
        UpdateAllInstanceTransformBufferIfNeeded();
        
        //增加判断，如何allGrassPos为空，直接返回
        if (allGrassPos.Count == 0)
            return;
        // Camera cam = Camera.main;
        //寻找角色相机，如果不存在，则直接返回
        if(HCameraLayoutManager.Instance == null)
            return;
        Camera cam = HCameraLayoutManager.Instance.playerCamera;
        if (!cam)
            return;

        //=====================================================================================================
        // rough quick big cell frustum culling in CPU first
        //=====================================================================================================
        //清空可见单元格ID列表。这个列表用于存储经过CPU视锥体裁剪后仍然可见的单元格的ID。
        visibleCellIDList.Clear();//fill in this cell ID list using CPU frustum culling first
        

        //Do frustum culling using per cell bound
        //https://docs.unity3d.com/ScriptReference/GeometryUtility.CalculateFrustumPlanes.html
        //https://docs.unity3d.com/ScriptReference/GeometryUtility.TestPlanesAABB.html
        float cameraOriginalFarPlane = cam.farClipPlane;
        // 设置摄像机的远裁剪面，这样可以控制草地的绘制距离。
        cam.farClipPlane = drawDistance;//allow drawDistance control    
        //计算摄像机的视锥体平面，用于后续的视锥体裁剪。
        GeometryUtility.CalculateFrustumPlanes(cam, cameraFrustumPlanes);//Ordering: [0] = Left, [1] = Right, [2] = Down, [3] = Up, [4] = Near, [5] = Far
        //恢复摄像机的远裁剪面。
        cam.farClipPlane = cameraOriginalFarPlane;//revert far plane edit

        //slow loop
        //TODO: (A)replace this forloop by a quadtree test?
        //TODO: (B)convert this forloop to job+burst? (UnityException: TestPlanesAABB can only be called from the main thread.)
        Profiler.BeginSample("CPU cell frustum culling (heavy)");
        
        //调度compute shader的。对于每个可见的单元格，都会调度一次compute shader，进行GPU视锥体裁剪。
        //compute shader会把所有可见的草地实例的ID写入到visibleInstancesOnlyPosWSIDBuffer缓冲区中。
        for (int i = 0; i < cellPosWSsList.Length; i++)
        {
            //create cell bound
            //为每个单元格创建一个边界框（Bounds）。
            Vector3 centerPosWS = new Vector3 (i % cellCountX + 0.5f, 0, i / cellCountX + 0.5f);
            // 将单元格的中心位置从单元格空间转换到世界空间
            centerPosWS.x = Mathf.Lerp(minX, maxX, centerPosWS.x / cellCountX);
            centerPosWS.z = Mathf.Lerp(minZ, maxZ, centerPosWS.z / cellCountZ);
            // 计算每个单元格在世界空间中的大小 todo: 优化：可以挪到外面
            Vector3 sizeWS = new Vector3(Mathf.Abs(maxX - minX) / cellCountX,0,Mathf.Abs(maxX - minX) / cellCountX);
            // 创建一个边界框，中心位置为单元格的中心位置，大小为单元格的大小
            Bounds cellBound = new Bounds(centerPosWS, sizeWS);
            // 检查边界框是否在摄像机的视锥体内
            if (GeometryUtility.TestPlanesAABB(cameraFrustumPlanes, cellBound))
            {
                // 如果边界框在视锥体内，那么这个单元格的ID就会被添加到`visibleCellIDList`列表中
                visibleCellIDList.Add(i);
            }
        }
        Profiler.EndSample();

        //=====================================================================================================
        // then loop though only visible cells, each visible cell dispatch GPU culling job once
        // at the end compute shader will fill all visible instance into visibleInstancesOnlyPosWSIDBuffer
        //首先，代码会遍历所有可见的单元格（cell）。对于每一个可见的单元格，代码会调度一次GPU视锥体裁剪任务。
        //这个任务是由一个计算着色器（Compute Shader）执行的，它会检查每一个草地实例是否在摄像机的视锥体内。
        //如果一个草地实例在视锥体内，那么这个实例就是可见的，需要被渲染。
        //这个计算着色器会把所有可见的草地实例的ID写入到一个名为visibleInstancesOnlyPosWSIDBuffer的缓冲区中。 
        //=====================================================================================================
        Matrix4x4 v = cam.worldToCameraMatrix;
        Matrix4x4 p = cam.projectionMatrix;
        Matrix4x4 vp = p * v;

        //将可见实例缓冲区的计数器设置为0   这个缓冲区用于存储视锥体裁剪后仍然可见的草地实例的ID。
        visibleInstancesOnlyPosWSIDBuffer.SetCounterValue(0);

        //set once only
        cullingComputeShader.SetMatrix("_VPMatrix", vp);
        cullingComputeShader.SetFloat("_MaxDrawDistance", drawDistance);

        //dispatch per visible cell
        //todo:可以用前缀和优化
        dispatchCount = 0;
        for (int i = 0; i < visibleCellIDList.Count; i++)
        {
            int targetCellFlattenID = visibleCellIDList[i];
            int memoryOffset = 0;
            //也就是说 计算这个单元格及其之前的所有单元格所拥有的草，加起来就是这个单元格的memoryOffset 
            for (int j = 0; j < targetCellFlattenID; j++)
            {
                //cellPosWSsList是一个列表的数组，
                //其中每个列表都包含了一个单元格（cell）中所有草地实例的世界空间位置（World Space Position）
                memoryOffset += cellPosWSsList[j].Count;
            }
            cullingComputeShader.SetInt("_StartOffset", memoryOffset); //culling read data started at offseted pos, will start from cell's total offset in memory
            //jobLength=目标单元格中草地实例的数量
            int jobLength = cellPosWSsList[targetCellFlattenID].Count;

            //============================================================================================
            //batch n dispatchs into 1 dispatch, if memory is continuous in allInstancesPosWSBuffer
            //是否要批量调度。如果内存是连续的，那么可以将多个调度合并成一个，这样可以提高性能。 
            if(shouldBatchDispatch)
            {
                while ((i < visibleCellIDList.Count - 1) && //test this first to avoid out of bound access to visibleCellIDList
                        (visibleCellIDList[i + 1] == visibleCellIDList[i] + 1))
                {
                    //if memory is continuous, append them together into the same dispatch call
                    jobLength += cellPosWSsList[visibleCellIDList[i + 1]].Count;
                    i++;
                }
            }
            //============================================================================================
            //调度了ComputeShader。这个调度会对每个单元格中的草地实例进行视锥体裁剪，并将可见的实例的ID写入到visibleInstancesOnlyPosWSIDBuffer缓冲区中
            if (jobLength > 0)
            {
                cullingComputeShader.Dispatch(0, Mathf.CeilToInt(jobLength / 64f), 1, 1); //disaptch.X division number must match numthreads.x in compute shader (e.g. 64)
                dispatchCount++;
            }
           
        }

        //====================================================================================
        // Final 1 big DrawMeshInstancedIndirect draw call 最后一次大规模的 DrawMeshInstancedIndirect 绘制调用。
        //====================================================================================
        // GPU per instance culling finished, copy visible count to argsBuffer, to setup DrawMeshInstancedIndirect's draw amount 
        //GPU对每个实例的剔除已完成，将可见计数复制到argsBuffer，以设置DrawMeshInstancedIndirect的绘制数量。
        ComputeBuffer.CopyCount(visibleInstancesOnlyPosWSIDBuffer, argsBuffer, 4);

        // Render 1 big drawcall using DrawMeshInstancedIndirect 使用DrawMeshInstancedIndirect渲染一次大规模的绘制调用。
        Bounds renderBound = new Bounds();
        renderBound.SetMinMax(new Vector3(minX, 0, minZ), new Vector3(maxX, 0, maxZ));//if camera frustum is not overlapping this bound, DrawMeshInstancedIndirect will not even render
        Graphics.DrawMeshInstancedIndirect(GetGrassMeshCache(), 0, instanceMaterial, renderBound, argsBuffer);
    }

    void OnDisable()
    {
        //release all compute buffers
        if (allInstancesPosWSBuffer != null)
            allInstancesPosWSBuffer.Release();
        allInstancesPosWSBuffer = null;

        if (visibleInstancesOnlyPosWSIDBuffer != null)
            visibleInstancesOnlyPosWSIDBuffer.Release();
        visibleInstancesOnlyPosWSIDBuffer = null;

        if (argsBuffer != null)
            argsBuffer.Release();
        argsBuffer = null;

        instance = null;
    }

    Mesh GetGrassMeshCache()
    {
        if (!cachedGrassMesh)
        {
            //if not exist, create a 3 vertices hardcode triangle grass mesh
            cachedGrassMesh = new Mesh();

            //single grass (vertices)
            Vector3[] verts = new Vector3[3];
            verts[0] = new Vector3(-0.25f, 0);
            verts[1] = new Vector3(+0.25f, 0);
            verts[2] = new Vector3(-0.0f, 1);
            //single grass (Triangle index)
            int[] trinagles = new int[3] { 2, 1, 0, }; //order to fit Cull Back in grass shader

            cachedGrassMesh.SetVertices(verts);
            cachedGrassMesh.SetTriangles(trinagles, 0);
        }

        return cachedGrassMesh;
    }

    //更新所有草地实例的变换缓冲区。如果草地实例的数量发生变化，或者草地实例的位置发生变化，那么就需要更新这个缓冲区。
    void UpdateAllInstanceTransformBufferIfNeeded()
    {
        //always update
        instanceMaterial.SetVector("_PivotPosWS", transform.position);
        //instanceMaterial.SetVector("_BoundSize", new Vector2(2, 2));
        instanceMaterial.SetVector("_BoundSize", new Vector2(transform.localScale.x, transform.localScale.z));
        //instanceMaterial指的是*InstancedIndirectGrass.shader：这是一个用于渲染草地的着色器。
        //它使用了**GPU Instancing**技术来批量渲染大量的草地，从而提高性能。这个着色器还包含了风的模拟，使得草地能够随风摇摆。
        
        //如果不需要更新缓冲区，则提前退出
        //early exit if no need to update buffer
        if (instanceCountCache == allGrassPos.Count &&
            argsBuffer != null &&
            allInstancesPosWSBuffer != null &&
            visibleInstancesOnlyPosWSIDBuffer != null)
            {
                return;
            }

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        Debug.Log("UpdateAllInstanceTransformBuffer (Slow)");

        ///////////////////////////
        // allInstancesPosWSBuffer buffer
        ///////////////////////////
        if (allInstancesPosWSBuffer != null)
            allInstancesPosWSBuffer.Release();
        
        //增加判断，如何allGrassPos为空，直接返回
        if (allGrassPos.Count == 0)
            return;
        
        allInstancesPosWSBuffer = new ComputeBuffer(allGrassPos.Count, sizeof(float)*3); //float3 posWS only, per grass

        if (visibleInstancesOnlyPosWSIDBuffer != null)
            visibleInstancesOnlyPosWSIDBuffer.Release();
        visibleInstancesOnlyPosWSIDBuffer = new ComputeBuffer(allGrassPos.Count, sizeof(uint), ComputeBufferType.Append); //uint only, per visible grass

        //函数会计算所有草地实例的位置的最小和最大值，这些值用于确定单元格的数量
        //find all instances's posWS XZ bound min max
        minX = float.MaxValue;
        minZ = float.MaxValue;
        maxX = float.MinValue;
        maxZ = float.MinValue;
        for (int i = 0; i < allGrassPos.Count; i++)
        {
            Vector3 target = allGrassPos[i];
            minX = Mathf.Min(target.x, minX);
            minZ = Mathf.Min(target.z, minZ);
            maxX = Mathf.Max(target.x, maxX);
            maxZ = Mathf.Max(target.z, maxZ);
        }

        //decide cellCountX,Z here using min max
        //each cell is cellSizeX x cellSizeZ
        cellCountX = Mathf.CeilToInt((maxX - minX) / cellSizeX); 
        cellCountZ = Mathf.CeilToInt((maxZ - minZ) / cellSizeZ);

        //init per cell posWS list memory
        cellPosWSsList = new List<Vector3>[cellCountX * cellCountZ]; //flatten 2D array
        for (int i = 0; i < cellPosWSsList.Length; i++)
        {
            cellPosWSsList[i] = new List<Vector3>();
        }

        //binning, put each posWS into the correct cell
        //把每个cell填上这个cell中的草实例
        for (int i = 0; i < allGrassPos.Count; i++)
        {
            Vector3 pos = allGrassPos[i];

            //find cellID
            int xID = Mathf.Min(cellCountX-1,Mathf.FloorToInt(Mathf.InverseLerp(minX, maxX, pos.x) * cellCountX)); //use min to force within 0~[cellCountX-1]  
            int zID = Mathf.Min(cellCountZ-1,Mathf.FloorToInt(Mathf.InverseLerp(minZ, maxZ, pos.z) * cellCountZ)); //use min to force within 0~[cellCountZ-1]

            cellPosWSsList[xID + zID * cellCountX].Add(pos);
        }

        //combine to a flatten array for compute buffer 组合为计算缓冲区的扁平数组
        int offset = 0;
        Vector3[] allGrassPosWSSortedByCell = new Vector3[allGrassPos.Count];
        for (int i = 0; i < cellPosWSsList.Length; i++)
        {
            for (int j = 0; j < cellPosWSsList[i].Count; j++)
            {
                allGrassPosWSSortedByCell[offset] = cellPosWSsList[i][j];
                offset++;
            }
        }

        allInstancesPosWSBuffer.SetData(allGrassPosWSSortedByCell);
        instanceMaterial.SetBuffer("_AllInstancesTransformBuffer", allInstancesPosWSBuffer);
        instanceMaterial.SetBuffer("_VisibleInstanceOnlyTransformIDBuffer", visibleInstancesOnlyPosWSIDBuffer);

        ///////////////////////////
        // Indirect args buffer
        ///////////////////////////
        if (argsBuffer != null)
            argsBuffer.Release();
        uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);

        args[0] = (uint)GetGrassMeshCache().GetIndexCount(0);
        args[1] = (uint)allGrassPos.Count;
        args[2] = (uint)GetGrassMeshCache().GetIndexStart(0);
        args[3] = (uint)GetGrassMeshCache().GetBaseVertex(0);
        args[4] = 0;

        argsBuffer.SetData(args);

        ///////////////////////////
        // Update Cache
        ///////////////////////////
        //update cache to prevent future no-op buffer update, which waste performance
        instanceCountCache = allGrassPos.Count;
        
        //set buffer
        cullingComputeShader.SetBuffer(0, "_AllInstancesPosWSBuffer", allInstancesPosWSBuffer);
        cullingComputeShader.SetBuffer(0, "_VisibleInstancesOnlyPosWSIDBuffer", visibleInstancesOnlyPosWSIDBuffer);
    }
}