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
    LevelNecessary, //通关所需要的物品
    RogueMoney,
}

public enum TreasureType
{
    CommonChest,
    GoodChest,
    VeryGoodChest,
    VeryVeryGoodChest,
    ChestKeepGiving,
    WangxiayitongRunChest,
}

public class RogueCharacterBaseAttribute
{
    public string characterId;
    public string characterName;
    public string characterChineseName;
    public string characterDescription;
    public float rogueMoveSpeed;
    public float rogueShootRate;
    public float rogueShootRange;
    public float rogueBulletDamage;
    public float rogueBulletSpeed;
    public int rogueCharacterHealth;
    public int rogueCharacterShield;
    public MyShootEnum RogueCharacterBaseWeapon;
    public int rogueStartXingqiong;
    public int rogueStartXinyongdian;
    public string rogueCharacterIconLink;
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

    public int starLevel;
    public bool couldBeExchanged; //暂时是是否可以被星际和平公司投资机器兑换
    public string itemKindChinese;

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

public class MessageBoxBaseStruct
{
    private string messageId;
    private string messageContent;
    private int messageType;
    private float messageShowTime;
    private string messageTransitionEffect;
    private string messageLink;
    
    #region Gets And Sets
    
    public string MessageLink
    {
        get { return messageLink; }
    }
    public string MessageId
    {
        get { return messageId; }
    }
    
    public string MessageContent
    {
        get { return messageContent; }
    }
    
    public int MessageType
    {
        get { return messageType; }
    }
    
    public float MessageShowTime
    {
        get { return messageShowTime; }
    }
    
    public string MessageTransitionEffect
    {
        get { return messageTransitionEffect; }
    }
    
    #endregion
    
    public MessageBoxBaseStruct(string id, string content, int type, float showTime, string transitionEffect, string link)
    {
        messageId = id;
        messageContent = content;
        messageType = type;
        messageShowTime = showTime;
        messageTransitionEffect = transitionEffect;
        messageLink = link;
    }

    public void SetMessage(string message)
    {
        messageContent = new string(message);
    }
}



