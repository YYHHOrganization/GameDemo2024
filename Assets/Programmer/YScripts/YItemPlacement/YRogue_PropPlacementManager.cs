using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YRogue_PropPlacementManager : MonoBehaviour
{
    
    [SerializeField, Header("道具的预制体")]
    private GameObject propPrefab;

    [SerializeField]
    [Header("需要放置的道具列表")]
    private List<ItemDataInfo> itemDataInfos;

    private GameObject itemDataParent;//物品父类
    Transform parent;

    // 放置物品
    public void SetData(YRogue_ItemPlacementHelper itemPlacementHelper,Transform parent)
    {
        // ClearData();
        this.parent = parent;

        foreach (var itemDataInfo in itemDataInfos)
        {
            bool useProbility = itemDataInfo.useProbility;
            if (useProbility)
            {
                //例如如果ProbilityIn100是20，那么就是有20%的概率出现这个物品，
                int probility = UnityEngine.Random.Range(0, 100);
                if (probility > itemDataInfo.ProbilityIn100)//意味着如果随机数大于20，那么就不生成这个物品 直接跳过
                {
                    continue;
                }
            }
            int count = UnityEngine.Random.Range(itemDataInfo.minQuantity, itemDataInfo.maxQuantity + 1);
            for (int i = 0; i < count; i++)
            {
                var position = itemPlacementHelper.GetItemPlacementPosition(itemDataInfo.itemData.placementType, 10, itemDataInfo.itemData.size, itemDataInfo.itemData.addOffset);
                if (position != null)
                {
                    Vector3 pos3From2 = new Vector3(position.Value.x, 0, position.Value.y);
                    SetIteamData(pos3From2, itemDataInfo);
                }
            }
        }
    }

    //清空物品
    private void ClearData()
    {
        itemDataParent = GameObject.Find("ItemDataParent");
        //清空物品
        if (itemDataParent) DestroyImmediate(itemDataParent);
        itemDataParent = new GameObject("ItemDataParent");
    }

    //放置物品
    private void SetIteamData(Vector3 position, ItemDataInfo itemDataInfo)
    {
        // 实例化道具对象
        // GameObject prop = Instantiate(propPrefab, position, Quaternion.identity);
        
        GameObject model = Instantiate(itemDataInfo.itemData.model, position, Quaternion.identity);
        GameObject prop = model;

        //绑定父级
        // prop.transform.SetParent(itemDataParent.transform);
        prop.transform.SetParent(parent);

        //修改名称
        prop.name = itemDataInfo.itemData.name;

        // SpriteRenderer propSpriteRenderer = prop.GetComponentInChildren<SpriteRenderer>();

        // 添加碰撞体
        // CapsuleCollider2D collider = propSpriteRenderer.gameObject.AddComponent<CapsuleCollider2D>();
        
        
        // collider.offset = Vector2.zero;
        // 根据道具大小设置碰撞体方向
        // if (itemDataInfo.itemData.size.x > itemDataInfo.itemData.size.y)
        // {
        //     collider.direction = CapsuleDirection2D.Horizontal;
        // }
        // 根据道具大小设置碰撞体大小
        // Vector2 size = new Vector2(itemDataInfo.itemData.size.x * 0.8f, itemDataInfo.itemData.size.y * 0.8f);
        // collider.size = size;

        // 设置道具的精灵图片
        // propSpriteRenderer.sprite = itemDataInfo.itemData.sprite;
        //调整精灵图片的位置
        // propSpriteRenderer.transform.localPosition = new Vector2(1, 1) * 0.5f;
    }
}
