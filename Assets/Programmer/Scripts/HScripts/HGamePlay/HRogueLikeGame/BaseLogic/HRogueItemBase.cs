using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
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
    

    public virtual void GetToBagAndShowEffects()
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
            SetActiveFalseAndDestroy(5f);
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
        SetActiveFalseAndDestroy(5f);
    }

    public void AddHeartOrShield(string funcParams)
    {
        //根据funcParams的内容，决定加血/加护盾，或者是加血量上限/加护盾上限
        string[] paramList = funcParams.Split(';');
        string attributeName = (string)paramList[0];
        int attributeValue = int.Parse(paramList[1]);
        if (HRoguePlayerAttributeAndItemManager.Instance.AddHeartOrShield(attributeName, attributeValue))
        {
            SetActiveFalseAndDestroy(5f);
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
        SetActiveFalseAndDestroy(5f);
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
        SetActiveFalseAndDestroy(5f);
    }

    public void SetBillboardEffect()
    {
        if (!getUI.GetComponentInParent<HRotateToPlayerCamera>()) return;
        getUI.GetComponentInParent<HRotateToPlayerCamera>().enabled = true;
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
        SetActiveFalseAndDestroy(5f);
    }
    
    # region 40~62

    public void SetEveryItemName(string funcParams)
    {
        foreach (var item in yPlanningTable.Instance.rogueItemBases)
        {
            item.Value.rogueItemNameShowDefault = bool.Parse(funcParams);
        }
        SetActiveFalseAndDestroy(5f);
    }

    public void AddEnemyHealth(string funcParams)
    {
        string[] paramList = funcParams.Split(';');
        float value = float.Parse(paramList[1]);
        string type = paramList[0];
        var enemy = SD_RogueEnemyCSVFile.Class_Dic;
        //遍历敌人表，对每个敌人的生命值进行操作
        if (type == "AddUpperHealth")
        {
            foreach (var enemyData in enemy)
            {
                string upperHealth = enemyData.Value.RogueEnemyStartHealth;
                int changeHealth = (int)(float.Parse(upperHealth) + value);
                if (changeHealth < 1) changeHealth = 1;
                enemyData.Value.RogueEnemyStartHealth = changeHealth.ToString();
            }
        }
        else if (type == "MultiplyUpperHealth")
        {
            foreach (var enemyData in enemy)
            {
                string upperHealth = enemyData.Value.RogueEnemyStartHealth;
                int changeHealth = (int)(float.Parse(upperHealth) * value);
                if (changeHealth < 1) changeHealth = 1;
                enemyData.Value.RogueEnemyStartHealth = changeHealth.ToString();
            }
        }
        SetActiveFalseAndDestroy(5f);
    }

    public void GetAllBlessWithKind(string funcParams)
    {
        HRoguePlayerAttributeAndItemManager.Instance.GiveOutRuanmeiItem(funcParams);
        SetActiveFalseAndDestroy(5f);
    }
    
    public void SetAttributeWithCertainLogic(string funcParams)
    {
        string[] paramList = funcParams.Split(';');
        string logic = paramList[0];
        int value = int.Parse(paramList[1]);
        Dictionary<string, float> characterAttributes =
            HRoguePlayerAttributeAndItemManager.Instance.characterValueAttributes;
        List<string> attributes = HRoguePlayerAttributeAndItemManager.Instance.attributesWithNoMoney;
        
        switch (logic)
        {
            case "AddAll":
                for (int i = 0; i < attributes.Count; i++)
                {
                    characterAttributes[attributes[i]] += value;
                    if(characterAttributes[attributes[i]] < 1.0f)  //角色的各种普通属性的最小值是1
                    {
                        characterAttributes[attributes[i]] = 1.0f;
                    }
                }
                break;
            case "AddMin":
                float minValue = float.MaxValue;
                int minIndex = -1;
                for (int i = 0; i < attributes.Count; i++)
                {
                    if (characterAttributes[attributes[i]] < minValue)
                    {
                        minValue = characterAttributes[attributes[i]];
                        minIndex = i;
                    }
                }
                characterAttributes[attributes[minIndex]] += value;
                if(characterAttributes[attributes[minIndex]] < 1.0f) 
                {
                    characterAttributes[attributes[minIndex]] = 1.0f;
                }
                break;
            case "AddMax":
                float maxValue = float.MinValue;
                int maxIndex = -1;
                for (int i = 0; i < attributes.Count; i++)
                {
                    if (characterAttributes[attributes[i]] > maxValue)
                    {
                        maxValue = characterAttributes[attributes[i]];
                        maxIndex = i;
                    }
                }
                characterAttributes[attributes[maxIndex]] += value;
                if(characterAttributes[attributes[maxIndex]] < 1.0f) 
                {
                    characterAttributes[attributes[maxIndex]] = 1.0f;
                }
                break;
                
            case "Avg":
                float sumValue = 0;
                for (int i = 0; i < attributes.Count; i++)
                {
                    sumValue += characterAttributes[attributes[i]];
                }
                float avgValue = sumValue / attributes.Count;
                for(int i = 0; i < attributes.Count; i++)
                {
                    characterAttributes[attributes[i]] = avgValue;
                    if(characterAttributes[attributes[i]] < 1.0f) 
                    {
                        characterAttributes[attributes[i]] = 1.0f;
                    }
                }

                break;
            
            case "Random":
                int randomIndex = UnityEngine.Random.Range(0, attributes.Count);
                characterAttributes[attributes[randomIndex]] += value;
                if(characterAttributes[attributes[randomIndex]] < 1.0f) 
                {
                    characterAttributes[attributes[randomIndex]] = 1.0f;
                }
                break;
                
        }
        HRoguePlayerAttributeAndItemManager.Instance.UpdateEverythingInAttributePanel();
        SetActiveFalseAndDestroy(5f);
        
    }

    public void Yongdongguguzhong(string funcParams)
    {
        Debug.Log("Yongdongguguzhong");
        //todo：还没有写完
    }

    public void SetShopItemPriceMultiply(string funcParams)
    {
        Debug.Log("SetShopItemPriceMultiply");
        //todo：还没有写完
    }

    public void SetCameraPostProcessingEffect(string funcParams)
    {
        string[] paramList = funcParams.Split(';');
        string funcName = paramList[0];
        float lastTime = float.Parse(paramList[1]);
        HPostProcessingFilters.Instance.SetPostProcessingWithNameAndTime(funcName,lastTime);
        SetActiveFalseAndDestroy(5f); 
    }
    
    public void Bishangshuangyan(string funcParams)
    {
        switch (funcParams)
        {
            case "Left":
                HRoguePlayerAttributeAndItemManager.Instance.ShowHeartAndShield(false);
                break;
            case "None":
                HRoguePlayerAttributeAndItemManager.Instance.ShowHeartAndShield(true);
                break;
            case "Both":
                HRoguePlayerAttributeAndItemManager.Instance.ShowHeartAndShield(false);
                break;
        }
        SetActiveFalseAndDestroy(0.5f);
    }

    public void GiveARandomItemWithIdRange(string funcParams)
    {
        string startId = funcParams.Split(':')[0];
        string endId = funcParams.Split(':')[1];
        int randomIndex = UnityEngine.Random.Range(int.Parse(startId), int.Parse(endId));
        string randomId = randomIndex.ToString();
        HRoguePlayerAttributeAndItemManager.Instance.RollingAnItemThenUseImmediately(randomId);
        SetActiveFalseAndDestroy(0.5f);
    }
    
    
    public void HurtEveryEnemyInRoom(int value)
    {
        List<GameObject> enemies =
            YRogue_RoomAndItemManager.Instance.currentRoom.GetComponent<YRouge_RoomBase>().Enemies;
        if (enemies!=null && enemies.Count > 0)  //当前是战斗房
        {
            
        }
    }
    
    #endregion
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //getUI.GetComponentInParent<HRotateToPlayerCamera>().enabled = true;
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
