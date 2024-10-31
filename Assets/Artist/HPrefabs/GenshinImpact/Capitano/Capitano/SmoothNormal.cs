using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothNormal: MonoBehaviour
{
    public bool testSmoothNormal = false;
    Mesh MeshNormalAverage(Mesh mesh)
    {
        //可以参考的文档：https://docs.unity3d.com/Manual/class-Mesh.html
        Dictionary<Vector3, List<int>> map = new Dictionary<Vector3, List<int>>();
        //在Unity的Mesh当中,vertices会把每个面的顶点都列出来,所以会有重复的顶点
        for (int v = 0; v < mesh.vertexCount; ++v)
        {
            if (!map.ContainsKey(mesh.vertices[v]))
            {
                map.Add(mesh.vertices[v], new List<int>());
            }
            map[mesh.vertices[v]].Add(v);  //记录哪些vertices中的顶点索引指的是同一个点,
            //因为有重复的顶点,所以一个顶点也会有很多法线记录的值,每个顶点最终的法线方向是代表这个点的重复法线方向的平均值
        }
        Vector3[] normals = mesh.normals;
        Vector3 normal;
        foreach(var p in map)
        {
            normal = Vector3.zero;
            foreach (var n in p.Value)
            {
                normal += mesh.normals[n];
            }
            normal /= p.Value.Count;
            foreach (var n in p.Value)
            {
                normals[n] = normal;
            }
        }
        mesh.normals = normals;
        return mesh;
    }
    private void Awake()
    {
        if (!testSmoothNormal) return;
        if (GetComponent<MeshFilter>())
        {
            Mesh tempMesh = (Mesh)Instantiate(GetComponent<MeshFilter>().sharedMesh);
            tempMesh = MeshNormalAverage(tempMesh);
            gameObject.GetComponent<MeshFilter>().sharedMesh = tempMesh;
        }
        if (GetComponent<SkinnedMeshRenderer>())
        {
            Mesh tempMesh = (Mesh)Instantiate(GetComponent<SkinnedMeshRenderer>().sharedMesh);
            tempMesh = MeshNormalAverage(tempMesh);
            gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh = tempMesh;
        }
    }
}