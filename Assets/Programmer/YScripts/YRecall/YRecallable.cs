using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 在物体（例如，Rigidbody）上添加一个新的脚本，比如YRecallable.
/// 这个脚本将负责跟踪物体的状态，并在需要时进行回溯
/// </summary>
public class YRecallable : MonoBehaviour
{
    private Rigidbody rb;
    //使用List<YRecallObject>，存储物体在每个FixedUpdate时的状态和时间戳。
    private List<YRecallObject> recallObjects;
    
    //绘制recallObjects中的轨迹
    // Variables for drawing the trajectory
    private LineRenderer lineRenderer;
    public float lineSegmentSpacing = 0.1f;

    public Material Choose_lineRendererMat;//回头可以弄成addlink
    public Material Recall_lineRendererMat;

    public string YRecallFinalStateMatAddLink = "YRecallFinalStateMat";

    // public Material CouldRecallObjectMat;
    private MeshRenderer meshRenderer;
    private Material[] objMaterials;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        recallObjects = new List<YRecallObject>();
        lineRenderer = GetComponent<LineRenderer>();
        meshRenderer = GetComponent<MeshRenderer>();
        objMaterials = meshRenderer.materials;
    }

    private bool isMoving = false;
    private Vector3 lastPosition;
    private Vector3 lastVelocity;

    private float lastMoveTime = 0f;
    // The time period to keep recording after the object stops moving
    private float stationaryTime = 0.5f; 
    private void FixedUpdate()
    {
        // Calculate the acceleration
        Vector3 acceleration = (rb.velocity - lastVelocity) / Time.fixedDeltaTime;

        // Check if the object is moving
        if (Vector3.Distance(rb.position, lastPosition) > 0f)
        {
            // If the object starts moving, clear the list
            if (!isMoving)
            {
                recallObjects.Clear();
                isMoving = true;
            }

            // Store the object's state
            recallObjects.Add(new YRecallObject
            {
                Position = rb.position,
                Rotation = rb.rotation,
                Velocity = rb.velocity,
                AngularVelocity = rb.angularVelocity,
                Force = rb.mass * acceleration, // Calculate the force using the acceleration
                TimeStamp = Time.time
            });

            // Update the last move time
            lastMoveTime = Time.time;
        }
        else if (Time.time - lastMoveTime <= stationaryTime)
        {
            // If the object is not moving, but the last move time is within the stationary time, keep recording
            recallObjects.Add(new YRecallObject
            {
                Position = rb.position,
                Rotation = rb.rotation,
                Velocity = rb.velocity,
                AngularVelocity = rb.angularVelocity,
                Force = rb.mass * acceleration, // Calculate the force using the acceleration
                TimeStamp = Time.time
            });
        }
        else
        {
            // If the object stops moving and the last move time is beyond the stationary time, stop recording
            isMoving = false;
        }

        // Update the last position and velocity
        lastPosition = rb.position;
        lastVelocity = rb.velocity;
    }

    private Coroutine RecallCoroutine;
    
    float duration = 10f;
    public void Recalling(float duration = 10f)
    {
        this.duration = duration;
        RecallCoroutine = StartCoroutine(Recall());
    }
    public IEnumerator Recall()
    {
        lineRenderer.material = Recall_lineRendererMat;
        SetRecallObjectMat(true);
        DrawRecallTail();
        
        // Create a copy of the list to iterate over
        List<YRecallObject> tempRecallObjects = new List<YRecallObject>(recallObjects);
        tempRecallObjects.Reverse();
        foreach (var recallObject in tempRecallObjects)
        {
            // Set the object's position, rotation, velocity, and angular velocity
            rb.position = recallObject.Position;
            rb.rotation = recallObject.Rotation;
            
            rb.velocity = recallObject.Velocity;
            rb.angularVelocity = recallObject.AngularVelocity;
            
            // Apply the inverse force
            rb.AddForce(-recallObject.Force, ForceMode.Force);
            
            //此处将已经经历过的position删掉，也就是linerenderer并不会绘制
            lineRenderer.positionCount -= 1;

            // Wait for the next physics update
            yield return new WaitForFixedUpdate();
        }

        // Clear the original list after the recall
        recallObjects.Clear();
        SetFinalStateShow(false);
        //如果此时还没被终止，应该是要停在原地，也就是此时物体不再移动，那么其gravity应该为0
        StopMoving();
        
        //按理说应该会中途被停止 
        yield return new WaitForSeconds(duration);
        
        EndRecall();
    }

    private void StopMoving()
    {
        // Stop the object from moving
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
    }


    public void EndRecall()
    {
        if(RecallCoroutine!=null)StopCoroutine(RecallCoroutine);
        SetCouldRecallObjectMat(false);
        
        //SetRecallObjectMat(false); 包含在下面了
        ClearRecallTail();
        
        rb.useGravity = true;
    }

    //选中时的轨迹
    public void ChooseRecallTail()
    {
        SetRecallObjectMat(true);
        
        lineRenderer.material = Choose_lineRendererMat;
        DrawRecallTail();
        
        //此时还应该将物体的最后一个时刻的形态显现出来，也就是物体最后回溯完的形态显现出来
        //该采用何种方式呢，初始化一个新的物体，然后将其形态设置为最后一个时刻的形态（位置和旋转）?
        //todo:
        SetFinalStateShow(true);
    }
    
    GameObject lastStateDisplay;
    private void SetFinalStateShow(bool isOn)
    {
        if (!isOn)
        {
            if(lastStateDisplay!=null)lastStateDisplay.SetActive(false);
            return;
        }
        // if(isOn)
        YRecallObject lastState = recallObjects[0];// Get the last state of the object
        //如果只有一个点，那么就不显示了，否则会互相穿插很丑
        // if (recallObjects.Count <= 1)
        //如果lastState的点和当前的点位置小于某个距离且旋转小于某个角度，那么就不显示了
        if (recallObjects.Count <= 1 
            || (Vector3.Distance(lastState.Position, rb.position) < 0.1f && Quaternion.Angle(lastState.Rotation, rb.rotation) < 5f))
        {
            return;
        }

        if (lastStateDisplay == null)
        {
            Material finalStateMat =
                Addressables.LoadAssetAsync<Material>(YRecallFinalStateMatAddLink).WaitForCompletion();
            // Create a new game object to display the last state of the object
            lastStateDisplay = new GameObject("LastStateDisplay");
            //设置为原物体的子物体的话就会随着它移动 错误的
            lastStateDisplay.transform.parent = transform.parent;
            lastStateDisplay.transform.localScale = transform.localScale;
            
            // Get the MeshFilter and MeshRenderer components from the original object
            MeshFilter originalMeshFilter = GetComponent<MeshFilter>();
            // MeshRenderer originalMeshRenderer = GetComponent<MeshRenderer>();

            // Add a MeshFilter and MeshRenderer components to the new game object
            MeshFilter newMeshFilter = lastStateDisplay.AddComponent<MeshFilter>();
            MeshRenderer newMeshRenderer = lastStateDisplay.AddComponent<MeshRenderer>();

            // Set the mesh of the new MeshFilter to the mesh of the original MeshFilter
            newMeshFilter.mesh = originalMeshFilter.mesh;

            
            // Create a new materials array with the same length as objMaterials
            Material[] newMaterials = new Material[objMaterials.Length];

            // Set each material in the new array to finalStateMat
            for (int i = 0; i < newMaterials.Length; i++)
            {
                newMaterials[i] = finalStateMat;
            }

            // Set the materials of the new MeshRenderer to the new array
            newMeshRenderer.materials = newMaterials;
            
            // meshRenderer.material.SetFloat("_isCouldRecall", 1);
            // meshRenderer.material.SetFloat("_isRecall", 0);
        }
        // Set the position and rotation of the new game object to the last state of the object
        lastStateDisplay.transform.position = lastState.Position;
        lastStateDisplay.transform.rotation = lastState.Rotation;
        lastStateDisplay.SetActive(true);
        
    }

    public void ClearRecallTail()
    {
        SetRecallObjectMat(false);
        
        //后面可以优化 画一次就行了
        lineRenderer.positionCount = 0;
        
        SetFinalStateShow(false);
    }
    private void DrawRecallTail()
    {
        lineRenderer.positionCount = recallObjects.Count;
        
        for (int i = 0; i < recallObjects.Count; i++)
        {
            lineRenderer.SetPosition(i, recallObjects[i].Position);
        }
    }

    void SetRecallObjectMat(bool isRecall)
    {
        //meshRenderer.material.SetFloat("_isRecall", isRecall?1:0);
        for (int i = 0; i < objMaterials.Length; i++)
        {
            objMaterials[i].SetFloat("_isRecall", isRecall?1:0);
        }
    }

    public void SetCouldRecall()
    {
        SetCouldRecallObjectMat(true);
    }
    void SetCouldRecallObjectMat(bool isCouldRecall)
    {
        //meshRenderer.material.SetFloat("_isCouldRecall", isCouldRecall?1:0);
        for (int i = 0; i < objMaterials.Length; i++)
        {
            objMaterials[i].SetFloat("_isCouldRecall", isCouldRecall?1:0);
        }
    }
}