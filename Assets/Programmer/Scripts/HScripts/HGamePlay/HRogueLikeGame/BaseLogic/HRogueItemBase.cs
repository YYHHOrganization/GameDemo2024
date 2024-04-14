using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HRogueItemBase : MonoBehaviour
{
    //肉鸽玩法的物品道具基类，其他物品道具继承于这个基类
    private string itemId;
    private RogueItemBaseAttribute rogueItemBaseAttribute;

    private GameObject getUI;

    private bool isPickedUp;

    private TMP_Text itemChineseName;
    private bool canShowName = false;

    //如果是商店物品
    private bool isShop = false;
    //购买的货币
    private string buyCurrency;
    //货币数量 即价格
    private int howMuch;
    
    
    public void SetItemIDAndShow(string id, RogueItemBaseAttribute rogueItemAttribute)
    {
        itemId = id;
        rogueItemBaseAttribute = rogueItemAttribute;
        getUI = transform.Find("ShowCanvas/Panel").gameObject;
        getUI.gameObject.SetActive(false);
        isPickedUp = false;
        itemChineseName = getUI.transform.GetChild(1).GetComponent<TMP_Text>();
    }
    public void SetItemIDAndShow(string id, RogueItemBaseAttribute rogueItemAttribute,bool isShop,string buyCurrency,int howMuch)
    {
        itemId = id;
        rogueItemBaseAttribute = rogueItemAttribute;
        getUI = transform.Find("ShowCanvas/Panel").gameObject;
        getUI.gameObject.SetActive(false);
        isPickedUp = false;
        itemChineseName = getUI.transform.GetChild(1).GetComponent<TMP_Text>();
        this.isShop = isShop;
        this.buyCurrency = buyCurrency;
        this.howMuch = howMuch;
    }
    

    protected virtual void GetToBagAndShowEffects()
    {
        if (rogueItemBaseAttribute.rogueItemShowInBag) //需要显示在背包当中
        {
            HItemCounter.Instance.AddItemInRogue(itemId, 1);
        }
        //根据主动道具还是被动道具决定使用效果
        if (rogueItemBaseAttribute.rogueItemKind == "Negative") //被动道具，立刻生效
        {
            string funcName = rogueItemBaseAttribute.rogueItemFunc;
            string funcParams = rogueItemBaseAttribute.rogueItemFuncParams;
            UseNegativeItem(funcName, funcParams);
            HMessageShowMgr.Instance.ShowMessage("ROGUE_USE_NEGATIVE_ITEM", "你使用了被动道具——"+rogueItemBaseAttribute.itemChineseName);
        }
        else if (rogueItemBaseAttribute.rogueItemKind == "Positive")
        {
            SetActiveFalseAndDestroy(0.5f);
        }
    }

    private void SetActiveFalseAndDestroy(float time = 0f)
    {
        this.gameObject.SetActive(false);
        Destroy(this.gameObject, time);
    }

    protected virtual void UseNegativeItem(string funcName, string funcParams)
    {
        //利用反射调用函数funcName，传递参数funcParams
        System.Reflection.MethodInfo method = this.GetType().GetMethod(funcName);
        method.Invoke(this, new object[] {funcParams});
    }
    
    public void AddAttributeValue(string funcParams)
    {
        Debug.Log("AddAttributeValue");
        //根据funcParams的内容，将对应的属性值加上对应的数值
        string[] paramList = funcParams.Split(';');
        string attributeName = (string)paramList[0];
        float attributeValue = float.Parse(paramList[1]);
        HRoguePlayerAttributeAndItemManager.Instance.AddAttributeValue(attributeName, attributeValue);
        SetActiveFalseAndDestroy(0.5f);
    }

    public void AddHeartOrShield(string funcParams)
    {
        //根据funcParams的内容，决定加血/加护盾，或者是加血量上限/加护盾上限
        string[] paramList = funcParams.Split(';');
        string attributeName = (string)paramList[0];
        int attributeValue = int.Parse(paramList[1]);
        if (HRoguePlayerAttributeAndItemManager.Instance.AddHeartOrShield(attributeName, attributeValue))
        {
            SetActiveFalseAndDestroy(0.5f);
        }
    }

    private void DecreaseMoney(string attributeName, int value)
    {
        switch (attributeName)
        {
            case "RogueXingqiong":
                HItemCounter.Instance.RemoveItem("20000012", value);
                break;
            case "RogueXinyongdian":
                HItemCounter.Instance.RemoveItem("20000013", value);
                break;
        }
        SetActiveFalseAndDestroy(0.5f);
    }

    public void AddMoney(string funcParams)
    {
        string[] paramList = funcParams.Split(';');
        string attributeName = (string)paramList[0];
        int attributeValue = int.Parse(paramList[1]);
        if (attributeValue < 0)
        {
            DecreaseMoney(attributeName, attributeValue);
        }
        switch (attributeName)
        {
            case "RogueXingqiong":
                HItemCounter.Instance.AddItem("20000012", attributeValue);
                break;
            case "RogueXinyongdian":
                HItemCounter.Instance.AddItem("20000013", attributeValue);
                break;
        }
        SetActiveFalseAndDestroy(0.5f);
    }

    public void SetBillboardEffect()
    {
        if (rogueItemBaseAttribute.rogueItemIsImage)
        {
            getUI.GetComponentInParent<HRotateToPlayerCamera>().enabled = true;
        }
    }
    
    public void SetOrAddBulletType(string funcParams)
    {
        string[] paramList = funcParams.Split(';');
        string operation = (string)paramList[0];
        string bulletType = (string)paramList[1];
        if (operation == "Replace")
        {
            HRoguePlayerAttributeAndItemManager.Instance.ReplaceCurBulletType(bulletType);
        }
        else if (operation == "Add")
        {
            HRoguePlayerAttributeAndItemManager.Instance.AddBulletType(bulletType);
        }
        SetActiveFalseAndDestroy(0.5f);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            getUI.GetComponentInParent<HRotateToPlayerCamera>().enabled = true;
            getUI.gameObject.SetActive(true);
            if (itemChineseName)
            {
                if (rogueItemBaseAttribute.rogueItemNameShowDefault)
                {
                    itemChineseName.text = rogueItemBaseAttribute.itemChineseName;
                }
                else
                {
                    itemChineseName.text = "???";
                }
                
            }
            if (!isPickedUp && Input.GetKey(KeyCode.F))
            {
                if (isShop)
                {
                    //如果是商店，那么就要 点击了F直接购买，扣除钱
                    //同时应该判断钱是否够
                    OnShopItemPickUp(buyCurrency, howMuch);
                }
                else
                {
                    GetToBagAndShowEffects();
                    isPickedUp = true;
                }
               
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!isPickedUp && Input.GetKey(KeyCode.F))
            {
                if (isShop)
                {
                    //如果是商店，那么就要 点击了F直接购买，扣除钱
                    //同时应该判断钱是否够
                    OnShopItemPickUp(buyCurrency, howMuch);
                }
                else
                {
                    GetToBagAndShowEffects();
                    isPickedUp = true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            getUI.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnShopItemPickUp(string itemId, int count)
    {
        //如果是商店，那么就要 点击了F直接购买，扣除钱
        //同时应该判断钱是否够
        bool isEnough = HItemCounter.Instance.CheckAndRemoveItemInRogue(itemId, count);
        if (isEnough)
        {
            GetToBagAndShowEffects();
            isPickedUp = true;
        }
        else
        {
            HMessageShowMgr.Instance.ShowMessage("ROGUE_SHOP_NO_MONEY", "对不起我们不做慈善，你的钱不够啦！");
        }
        
    }
}
