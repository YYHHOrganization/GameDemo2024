using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YNoiseBasedGrassClusterPosDefine : MonoBehaviour
{
    public Texture2D noiseTexture; // 噪声图
    public Texture2D noiseTexture2; // 噪声图
    public int instanceCount = 200000; // 期望生成的草地实例数量
    public float drawDistance = 20; // 草地的绘制距离
    public float placementThreshold = 0.5f; // 用于决定是否在某点放置草地的阈值
    public float placementThresholdMin = 0.2f; // 用于决定是否在某点放置草地的阈值

    private int cacheCount = -1;

    void Start()
    {
        UpdatePosIfNeeded();
    }

    private void UpdatePosIfNeeded()
    {
        if (instanceCount == cacheCount) return;

        Debug.Log("UpdatePos (Slow)");

        List<Vector3> positions = new List<Vector3>(instanceCount);
        GenerateGrassFromNoiseMap(positions);

        InstancedIndirectGrassRenderer.instance.allGrassPos = positions;
        cacheCount = positions.Count;
    }

    private void GenerateGrassFromNoiseMap(List<Vector3> positions)
    {
        float scaleX = transform.localScale.x;
        float scaleZ = transform.localScale.z;
        
        //二噪声图相乘
        

        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int z = 0; z < noiseTexture.height; z++)
            {
                Color pixelColor = noiseTexture.GetPixel(x, z);
                Color pixelColor2 = noiseTexture2.GetPixel(x, z);
               
                // float value = pixelColor.r; // 使用灰度值作为亮度
                float value = pixelColor.r*pixelColor2.r; // 使用灰度值作为亮度

                //更改为有概率放置，如果亮度高于阈值则进行概率判断，值越高越有机会被放置：
                // if(value<placementThresholdMin)
                // {
                //     continue;
                // }
                // bool canPlace =false;
                // if(value>placementThreshold)
                // {
                //     canPlace = true;
                // }
                // else
                // {
                //     float randomValue = Random.Range(-0.1f, 0.1f);
                //     canPlace = value+randomValue < placementThreshold;
                //     //Debug.Log("canPlace: " + canPlace+" value: "+value+" randomValue: "+randomValue);
                // }
                
                if(value>placementThreshold)
                // if(canPlace==true)
                //if (value > placementThreshold)
                {
                    float worldX = (float)x / noiseTexture.width * scaleX * 2 - scaleX+Random.Range(-0.1f,0.1f);
                    float worldZ = (float)z / noiseTexture.height * scaleZ * 2 - scaleZ+Random.Range(-0.1f,0.1f);
                    Vector3 pos = new Vector3(worldX, 0, worldZ) + transform.position;
                    positions.Add(new Vector3(pos.x, pos.y, pos.z));
                }
            }
        }
    }
    
    
}