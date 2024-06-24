using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YRogue_ItemData : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[CreateAssetMenu(menuName = "Dungeon/ItemData")]
public class ItemData : ScriptableObject {
    [Header("物品图片")]
    public Sprite sprite;
    [Header("3d模型")] 
    public GameObject model;
    [Header("物品大小")]
    public Vector2Int size;
    [Header("放置类型枚举")]
    public PlacementType placementType;
    [Header("是否添加偏移量,可以有效防止物品阻塞路径")]
    public bool addOffset;
}

[Serializable]
public class ItemDataInfo {
    [SerializeField]
    public ItemData itemData;
    public int minQuantity;//最小数量
    public int maxQuantity;//最大数量

    public bool useProbility;
    public int ProbilityIn100;//概率 有多少概率出现这个东西
}


public class ItemDataInfoFromCsvTable
{
    public string theName;
    [Header("3d模型")] 
    public GameObject model;
    // public string modelAddressableLink;
    [Header("物品大小")]
    public int sizeX;
    public int sizeY;
    [Header("放置类型枚举")]
    public PlacementType placementType;
    [Header("是否添加偏移量,可以有效防止物品阻塞路径")]
    public bool addOffset;
    
    public int minQuantity;//最小数量
    public int maxQuantity;//最大数量

    public bool useProbility;
    public int ProbilityIn100;//概率 有多少概率出现这个东西
}
