using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Random = UnityEngine.Random;

public class YNoiseBasedGrassClusterPosDefine : MonoBehaviour
{
    public Texture2D noiseTexture; // 噪声图
    public Texture2D noiseTexture2; // 噪声图
    public int instanceCount = 200000; // 期望生成的草地实例数量
    public float drawDistance = 20; // 草地的绘制距离
    public float placementThreshold = 0.5f; // 用于决定是否在某点放置草地的阈值
    public float RogueplacementThreshold = 0.5f; // 用于决定是否在某点放置草地的阈值

    private int cacheCount = -1;

    void Start()
    {
        
        //UpdatePosIfNeeded();
        
        YTriggerEvents.OnEnterNewLevel += EnterNewLevel;
    }

    private void OnDestroy()
    {
        YTriggerEvents.OnEnterNewLevel -= EnterNewLevel;
    }

    void EnterNewLevel(object sender, YTriggerEventArgs e)
    {
        GetSpecialRoomAndSetGrassPos();
        
    }

    private void GetSpecialRoomAndSetGrassPos()
    {
        // YRouge_RoomBase specialRoom = YRogueDungeonManager.Instance.GetRoomBaseByType(RoomType.GameRoom);
        // if (specialRoom != null)
        // {
        //     //获取这个房间的真实宽高
        //     
        // }

        float Rwidth = 0;
        float Rheight = 0;
        
        YRogueDungeonManager.Instance.GetRoomBasePosByType(RoomType.GameRoom,out Vector3 RoomCenter,out Rwidth,out Rheight);
        //将这个东西移动到parentTransform的位置
        
        //在肉鸽房间中初始化草地的位置，后面需要改为在特定肉鸽层中初始化草地的位置,根据房间的宽高来初始化草地的位置
        Vector3 originPos = RoomCenter+new Vector3(0,0.001f,0);
        
        //将这个东西移动到parentTransform的位置
       
        GameObject newGrass = new GameObject("Grass");
        newGrass.transform.position = originPos;
        newGrass.transform.localScale = new Vector3(Rwidth, 1, Rheight);
        UpdatePos(newGrass.transform);
    }
    


    private void UpdateRoguePosGrass()
    {
        Vector3 originPos = YRogueDungeonManager.Instance.RogueDungeonOriginPos+new Vector3(50,0.001f,50);
        //将这个东西移动到parentTransform的位置
       
        GameObject newGrass = new GameObject("Grass");
        newGrass.transform.position = originPos;
        newGrass.transform.localScale = new Vector3(100, 1, 100);
        UpdatePos(newGrass.transform);
    }

    // private void Update()
    // {
    //     //测试如果点击了空格键，那么就会重新生成草地实例的位置。
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         Transform parentTransform = YPlayModeController.Instance.curCharacter.transform;
    //         GameObject newGrass = new GameObject("Grass");
    //         newGrass.transform.position = parentTransform.position+new Vector3(0,0.01f,0);
    //         newGrass.transform.localScale = new Vector3(20, 1, 20);
    //         UpdatePos(newGrass.transform);
    //     }
    // }
    private void UpdatePos(Transform parentTransform)
    {
        //将这个东西移动到parentTransform的位置
        transform.position = parentTransform.position;
        List<Vector3> positions = new List<Vector3>(instanceCount);
        RogueGenerateGrassFromNoiseMap4(positions,parentTransform);
        Debug.Log("GrassCount￥￥￥￥￥: " + positions.Count);
        InstancedIndirectGrassRenderer.instance.ReSetGrass();
        InstancedIndirectGrassRenderer.instance.allGrassPos = positions;
        cacheCount = positions.Count;
    }


    private void UpdatePosIfNeeded()
    {
        if (instanceCount == cacheCount) return;

        Debug.Log("UpdatePos (Slow)");

        List<Vector3> positions = new List<Vector3>(instanceCount);
        GenerateGrassFromNoiseMap3(positions);
        Debug.Log("GrassCount￥￥￥￥￥: " + positions.Count);

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
    private void GenerateGrassFromNoiseMap2(List<Vector3> positions)
    {
        float scaleX = transform.localScale.x;
        float scaleZ = transform.localScale.z;
        float worldWidth = scaleX * 2;
        float worldHeight = scaleZ * 2;

        for (float x = 0; x < worldWidth; x += worldWidth / noiseTexture.width)
        {
            for (float z = 0; z < worldHeight; z += worldHeight / noiseTexture.height)
            {
                float noiseX = x / worldWidth * noiseTexture.width;
                float noiseZ = z / worldHeight * noiseTexture.height;

                Color pixelColor = noiseTexture.GetPixel((int)noiseX, (int)noiseZ);
                Color pixelColor2 = noiseTexture2.GetPixel((int)noiseX, (int)noiseZ);
                float value = pixelColor.r * pixelColor2.r;

                if (value > placementThreshold)
                {
                    float worldX = x - scaleX + Random.Range(-0.1f, 0.1f);
                    float worldZ = z - scaleZ + Random.Range(-0.1f, 0.1f);
                    Vector3 pos = new Vector3(worldX, 0, worldZ) + transform.position;
                    positions.Add(new Vector3(pos.x, pos.y, pos.z));
                }
            }
        }
    }
    
    public float sampleInterval = 0.1f; // 采样间隔
    private void GenerateGrassFromNoiseMap3(List<Vector3> positions,Transform parent = null)
    {
        float scaleX = parent == null ? transform.localScale.x : parent.localScale.x;
        float scaleZ = parent == null ? transform.localScale.z : parent.localScale.z;
        float worldWidth = scaleX * 2;
        float worldHeight = scaleZ * 2;

        for (float x = 0; x < worldWidth; x += sampleInterval)
        {
            for (float z = 0; z < worldHeight; z += sampleInterval)
            {
                float noiseX = x / worldWidth * noiseTexture.width;
                float noiseZ = z / worldHeight * noiseTexture.height;

                Color pixelColor = noiseTexture.GetPixel((int)noiseX, (int)noiseZ);
                Color pixelColor2 = noiseTexture2.GetPixel((int)noiseX, (int)noiseZ);
                float value = pixelColor.r * pixelColor2.r;

                if (value > placementThreshold)
                {
                    float worldX = x - scaleX + Random.Range(-0.1f, 0.1f);
                    float worldZ = z - scaleZ + Random.Range(-0.1f, 0.1f);
                    Vector3 pos = new Vector3(worldX, 0, worldZ) + (parent == null ? transform.position : parent.position);
                    positions.Add(new Vector3(pos.x, pos.y, pos.z));
                }
            }
        }
    }
    
    //这个只用一张texture
    private void RogueGenerateGrassFromNoiseMap4(List<Vector3> positions,Transform parent = null)
    {
        float scaleX = parent == null ? transform.localScale.x : parent.localScale.x;
        float scaleZ = parent == null ? transform.localScale.z : parent.localScale.z;
        float worldWidth = scaleX * 2;
        float worldHeight = scaleZ * 2;

        for (float x = 0; x < worldWidth; x += sampleInterval)
        {
            for (float z = 0; z < worldHeight; z += sampleInterval)
            {
                float noiseX = x / worldWidth * noiseTexture.width;
                float noiseZ = z / worldHeight * noiseTexture.height;

                Color pixelColor = noiseTexture.GetPixel((int)noiseX, (int)noiseZ);
                
                float value = pixelColor.r ;

                if (value > RogueplacementThreshold)
                {
                    float worldX = x - scaleX + Random.Range(-0.1f, 0.1f);
                    float worldZ = z - scaleZ + Random.Range(-0.1f, 0.1f);
                    Vector3 pos = new Vector3(worldX, 0, worldZ) + (parent == null ? transform.position : parent.position);
                    positions.Add(new Vector3(pos.x, pos.y, pos.z));
                }
            }
        }
    }
}