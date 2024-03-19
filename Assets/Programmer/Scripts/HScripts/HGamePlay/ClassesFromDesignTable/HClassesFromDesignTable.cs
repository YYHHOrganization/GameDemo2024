using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enum ItemType
public enum ItemType
{
    Money,
    Treasure,
    Character,
    CommonItem,
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
    public bool isExpensive; //贵重的物体会在UI中间显示，比如珍贵宝箱的所有物品应该都会UI中间显示，单独逻辑
    
    public string UIIconLink;
    public string description;
    
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

    private string randomItemLogicString;
    private string randomItemAndCountString;
    public string UILayoutType;
    
    //属性
    public string FixedItemString
    {
        get { return fixedItemString; }
    }
    public string FixedNumString
    {
        get { return fixedNumString; }
    }
    public string RandomItemLogicString
    {
        get { return randomItemLogicString; }
    }
    public string RandomItemAndCountString
    {
        get { return randomItemAndCountString; }
    }
    
    public void SetFixedItems(string items, string nums)
    {
        //在刚开始游戏的时候，先把所有的宝箱的奖励以字符串读出来，不做处理，等到真正开宝箱的时候再做处理，不知能否省内存占用？
        fixedItemString = items;
        fixedNumString = nums;
    }
    
    public void SetRandomItems(string randomLogic, string randomItemAndCount)
    {
        randomItemLogicString = randomLogic;
        randomItemAndCountString = randomItemAndCount;
    }
    
}



