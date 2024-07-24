using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteAlways]
public class YClusterGrassPosDefine : MonoBehaviour
{
    [Range(1, 40000000)]
    public int instanceCount = 200000;//草地实例的数量
    public float drawDistance = 20;//草地实例的绘制距离

    public int numberOfClusters = 20; // Number of clusters
    public float minClusterRadius = 2f;
    public float maxClusterRadius = 5f;
    
    private int cacheCount = -1;

    private List<Vector3> clusterCenters = new List<Vector3>();
    float scaleX = 0;
    float scaleZ = 0;
    // Start is called before the first frame update
    void Start()
    {
        scaleX = transform.localScale.x-maxClusterRadius ;
        scaleZ = transform.localScale.z-maxClusterRadius ;
        UpdatePosIfNeeded();
        
    }

    private void UpdatePosIfNeeded()
    {
        //首先检查instanceCount是否发生了变化，如果没有变化则直接返回。
        if (instanceCount == cacheCount)
            return;

        Debug.Log("UpdatePos (Slow)");

        // 如果有变化，那么就会生成新的草地实例的位置，并将这些位置发送给InstancedIndirectGrassRenderer类的实例。 
        //这里只在这一块内进行渲染
        //same seed to keep grass visual the same
        // UnityEngine.Random.InitState(123);

        //auto keep density the same
        // float scale = Mathf.Sqrt((instanceCount / 4)) / 2f;
        
        // transform.localScale = new Vector3(scale, transform.localScale.y, scale);
        transform.localScale = new Vector3(scaleX, transform.localScale.y, scaleZ);

        //////////////////////////////////////////////////////////////////////////
        //can define any posWS in this section, random is just an example
        //////////////////////////////////////////////////////////////////////////
        List<Vector3> positions = new List<Vector3>(instanceCount);
        
        GenerateClusters(positions);

        //send all posWS to renderer
        InstancedIndirectGrassRenderer.instance.allGrassPos = positions;
        cacheCount = positions.Count;
    }

    private void GenerateClusters(List<Vector3> positions)
    {
        while (clusterCenters.Count < numberOfClusters)
        {
            Vector3 potentialCenter = new Vector3(
                UnityEngine.Random.Range(-scaleX, scaleX),
                0,
                UnityEngine.Random.Range(-scaleZ, scaleZ)
            ) + transform.position;

            if (IsFarEnoughFromExistingCenters(potentialCenter))
            {
                clusterCenters.Add(potentialCenter);
                FillCluster(potentialCenter, positions);
            }
        }
    }

    private bool IsFarEnoughFromExistingCenters(Vector3 center)
    {
        foreach (var existingCenter in clusterCenters)
        {
            if (Vector3.Distance(existingCenter, center) < minClusterRadius * 2) return false;
        }
        return true;
    }

    private void FillCluster(Vector3 center, List<Vector3> positions)
    {
        float clusterRadius = UnityEngine.Random.Range(minClusterRadius, maxClusterRadius);
        int instancesPerCluster = instanceCount / numberOfClusters;
        for (int i = 0; i < instancesPerCluster; i++)
        {
            Vector3 offset = Random.insideUnitCircle * clusterRadius;
            Vector3 pos = new Vector3(offset.x, 0, offset.y) + center;
            positions.Add(new Vector3(pos.x, pos.y, pos.z));
        }
    }
}
