using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enum ItemType
public enum ItemType
{
    Money,
    Treasure,
    Character,
}

public enum TreasureType
{
    CommonChest,
    GoodChest,
    VeryGoodChest,
    VeryVeryGoodChest,
}

public class HOpenWorldItemStruct
{
    public int id;
    public string itemName;
    public string chineseName;
    public ItemType itemType;
}

public class HOpenWorldTreasureStruct
{
    public int id;
    private List<HOpenWorldItemStruct> fixedItems;
    private List<HOpenWorldItemStruct> randomItems;
    private string fixedItemString;
    private string fixedNumString;
    public TreasureType treasureType;
    public string addressableLink;
    
    public void SetFixedItems(string items, string nums)
    {
        //在刚开始游戏的时候，先把所有的宝箱的奖励以字符串读出来，不做处理，等到真正开宝箱的时候再做处理，不知能否省内存占用？
        fixedItemString = items;
        fixedNumString = nums;
    }

    public void GiveoutTreasures()
    {
        //此时要解析字符串，并生成奖励
        string [] items = fixedItemString.Split(';');
        string [] nums = fixedNumString.Split(';');
        for (int i = 0; i < items.Length; i++)
        {
            string itemId = items[i];
            string itemName = yPlanningTable.Instance.worldItems[itemId].chineseName;
            string itemNum = nums[i];
            Debug.Log("宝箱类型是： " + treasureType + ", 开出了 "+ itemNum + "个" + itemName);
        }
        Debug.Log("====================================================");
        
    }
}

