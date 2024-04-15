using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class HRogueItemFuncUtility : MonoBehaviour
{
    //单例模式
    private static HRogueItemFuncUtility instance;
    public static HRogueItemFuncUtility Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new HRogueItemFuncUtility();
            }
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }
    
    private Dictionary<string, string> enterNewRoomEffects = new Dictionary<string, string>();
    private Dictionary<string, int> enterNewRoomEffectsCounter = new Dictionary<string, int>();
    private Dictionary<string, int> enterNewRoomPositiveItemCounter = new Dictionary<string, int>();
    private Dictionary<string, string> positiveItemEffects = new Dictionary<string, string>();
    private bool couldUsePositiveScreenItem = false;
    
    //根据道具的功能字符串，返回对应的功能
    public void UseNegativeItem(string funcName, string funcParams)
    {
        //利用反射调用函数funcName，传递参数funcParams
        System.Reflection.MethodInfo method = this.GetType().GetMethod(funcName);
        method.Invoke(this, new object[] {funcParams});
    }

    public void UsePositiveItemInBag(string funcName, string funcParams)
    {
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
    }

    public void AddHeartOrShield(string funcParams)
    {
        //根据funcParams的内容，决定加血/加护盾，或者是加血量上限/加护盾上限
        string[] paramList = funcParams.Split(';');
        string attributeName = (string)paramList[0];
        int attributeValue = int.Parse(paramList[1]);
        HRoguePlayerAttributeAndItemManager.Instance.AddHeartOrShield(attributeName, attributeValue);
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
    }
    
    # region 40~62

    public void SetEveryItemName(string funcParams)
    {
        foreach (var item in yPlanningTable.Instance.rogueItemBases)
        {
            item.Value.rogueItemNameShowDefault = bool.Parse(funcParams);
        }
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
    }

    public void GetAllBlessWithKind(string funcParams)
    {
        HRoguePlayerAttributeAndItemManager.Instance.GiveOutRuanmeiItem(funcParams);
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
        
    }

    private bool firstRegistEnterNewRoomFunc = true;
    private bool firstRegistEnterNewRoomPositiveFunc = true;
    public void RegisterEnterNewRoomFunc(string funcParams)
    {
        string registerFunc = funcParams.Split('!')[0];
        string Funcparams = funcParams.Split('!')[1];
        if (firstRegistEnterNewRoomFunc)
        {
            YTriggerEvents.OnEnterRoomType += EnterNewRoomEffect;
            firstRegistEnterNewRoomFunc = false;
        }
        
        enterNewRoomEffects.Add(registerFunc, Funcparams);
    }
    
    public void RegisterEnterNewRoomPositiveFuncWithCounter(string funcName, string funcParams)
    {
        if (enterNewRoomPositiveItemCounter.ContainsKey(funcName)) return;
        enterNewRoomPositiveItemCounter.Add(funcName, 0);
        if (firstRegistEnterNewRoomPositiveFunc)
        {
            YTriggerEvents.OnEnterRoomType += EnterNewRoomForPositiveItem;
            firstRegistEnterNewRoomPositiveFunc = false;
        }
        
        positiveItemEffects.Add(funcName, funcParams);
    }
    
    private void EnterNewRoomForPositiveItem(object sender,YTriggerEnterRoomTypeEventArgs e)
    {
        string funcName = HRoguePlayerAttributeAndItemManager.Instance.CurScreenPositiveFunc;
        enterNewRoomPositiveItemCounter[funcName]++;
        Debug.Log("EnterNewRomm!! " + funcName + "    " + enterNewRoomPositiveItemCounter[funcName]);
    }
    
    public void YongdongguguzhongEffect(string funcParams)
    {
        //这里写扣除信用点的效果
        string[] parameters = funcParams.Split(";");
        string moneyType = parameters[0];
        float multiplier = float.Parse(parameters[1]);
        int decreaseMoney = (int)HRoguePlayerAttributeAndItemManager.Instance.characterValueAttributes["RogueXinyongdian"];
        DecreaseMoney(moneyType, (int)(decreaseMoney * (1-multiplier)));
    }

    // public bool canUsePositiveScreenItem()
    // {
    //     string curPositiveFunc = HRoguePlayerAttributeAndItemManager.Instance.CurScreenPositiveFunc;
    //     if(enterNewRoomPositiveItemCounter[curPositiveFunc])
    // }

    //当角色释放这个技能的时候，就会触发这个函数
    public void ReleasePositiveScreenItem()  //释放所有的屏幕上的主动道具技能
    {
        foreach (var effect in positiveItemEffects)
        {
            System.Reflection.MethodInfo method = this.GetType().GetMethod(effect.Key);
            method.Invoke(this, new object[] {effect.Value});
        }
    }

    public void HeiyuanbaihuaEffect(string funcParams)
    {
        int heiyuanbaihuaUseCnt = enterNewRoomPositiveItemCounter["HeiyuanbaihuaEffect"];
        Debug.Log("Heiyuanbaihua!! " + heiyuanbaihuaUseCnt);
        if (heiyuanbaihuaUseCnt < 5)
        {
            return;
        }
        else
        {
            enterNewRoomPositiveItemCounter["HeiyuanbaihuaEffect"] = 0;
            int randomNum = Random.Range(0, 100);
            if (randomNum <= 80) //恢复半心，20%扣除半心
            {
                AddHeartOrShield("Health;1");
            }
            else
            {
                AddHeartOrShield("Health;-1");
            }
        }
    }

    private void EnterNewRoomEffect(object sender,YTriggerEnterRoomTypeEventArgs e)
    {
        foreach (var effect in enterNewRoomEffects)
        {
            System.Reflection.MethodInfo method = this.GetType().GetMethod(effect.Key);
            method.Invoke(this, new object[] {effect.Value});
        }
        //如果有计数器的效果，就要对计数器进行操作
        // foreach (var effect in enterNewRoomEffectsCounter)
        // {
        //     enterNewRoomEffectsCounter[effect.Key]++;
        // }
        
    }

    public void SetShopItemPriceMultiply(string funcParams)
    {
        string priceId = funcParams.Split(';')[0];
        float multiplyValue = float.Parse(funcParams.Split(';')[1]);
        List<YRouge_RoomBase> room = YRogueDungeonManager.Instance.GetRoomBaseList();
        foreach (var roomBase in room)
        {
            if (roomBase.RoomType == RoomType.ShopRoom)
            {
                YRouge_ShopRoom shopRoom = roomBase.GetComponent<YRouge_ShopRoom>();
                shopRoom.UpdatePrices(priceId, multiplyValue);
            }
        }
    }

    public void SetCameraPostProcessingEffect(string funcParams)
    {
        int randomNumber = Random.Range(0, 100);
        if (randomNumber <= 80) return;
        string[] paramList = funcParams.Split(';');
        string funcName = paramList[0];
        float lastTime = float.Parse(paramList[1]);
        HPostProcessingFilters.Instance.SetPostProcessingWithNameAndTime(funcName,lastTime);
    }
    
    public void Bishangshuangyan(string funcParams)
    {
        switch (funcParams)
        {
            case "Left":
                HRoguePlayerAttributeAndItemManager.Instance.ShowHeartAndShield(false);
                break;
            case "Right": //无法显示地图
                ShowOrHideAllMap(false);
                break;
            case "None": // 显示全部的地图，被遮挡的部分
                HRoguePlayerAttributeAndItemManager.Instance.ShowHeartAndShield(true);
                ShowOrHideAllMap(true);
                break;
            case "Both":
                HRoguePlayerAttributeAndItemManager.Instance.ShowHeartAndShield(false);
                ShowOrHideAllMap(false);
                break;
        }
    }

    private void ShowOrHideAllMap(bool isShow)
    {
        if (isShow)
        {
            HCameraLayoutManager.Instance.SetLittleMapCamera(true);
            List<YRouge_RoomBase> rooms = YRogueDungeonManager.Instance.GetRoomBaseList();
            foreach (var room in rooms)
            {
                if(room && room.GetComponent<YRouge_RoomBase>())
                {
                    room.GetComponent<YRouge_RoomBase>().RoomLittleMapMask.SetActive(false);
                }
            }
        }
        else
        {
            //直接把地图隐藏掉
            HCameraLayoutManager.Instance.SetLittleMapCamera(false);
        }
        
    }

    public void GiveARandomItemWithIdRange(string funcParams)
    {
        string startId = funcParams.Split('!')[0];
        string endId = funcParams.Split('!')[1];
        int randomIndex = UnityEngine.Random.Range(int.Parse(startId), int.Parse(endId));
        string randomId = randomIndex.ToString();
        HRoguePlayerAttributeAndItemManager.Instance.RollingAnItemThenUseImmediately(randomId);
    }
    
    
    public void HurtEveryEnemyInRoom(int value)
    {
        List<GameObject> enemies =
            YRogue_RoomAndItemManager.Instance.currentRoom.GetComponent<YRouge_RoomBase>().Enemies;
        if (enemies!=null && enemies.Count > 0)  //当前是战斗房
        {
            Debug.Log("HurtEveryEnemyInRoom");
        }
    }
    
    
    #endregion
    
    
    
    //以下是背包当中主动道具的逻辑
    public void ShowAllNegativeItemName(string funcParams)
    {
        Debug.Log("now we are in ShowAllNegativeItemNameFunc");
        foreach (var item in yPlanningTable.Instance.rogueItemBases)
        {
            if (item.Value.rogueItemKind == "Negative")
            {
                item.Value.rogueItemNameShowDefault = true;
            }
        }
    }
    
    public void ShowEffectWithNameAndTime(string effectNameAndTime)
    {
        string[] effectNameAndTimeArray = effectNameAndTime.Split(';');
        string effectName = effectNameAndTimeArray[0];
        float effectTime = float.Parse(effectNameAndTimeArray[1]);
        //用反射找到effectName对应的函数
        System.Reflection.MethodInfo method = this.GetType().GetMethod(effectName);
        StopCoroutine((IEnumerator)method.Invoke(this, new object[] {effectTime}));
        StartCoroutine((IEnumerator)method.Invoke(this, new object[] {effectTime}));
    }

    public void Heiyuanbaihua(string funcParams)
    {
        //todo:还未完成
        Debug.Log("Heiyuanbaihua!!!!");
    }

    public void HurtEveryEnemyInRoom(string funcParams)
    {
        //todo:还未完成
        Debug.Log("HurtEveryEnemyInRoom!!!!");
    }
    
    public void FrozenRoomEnemy(string funcParams)
    {
        //todo:还未完成
        Debug.Log("FrozenRoomEnemy!!!!");
    }

    public IEnumerator RotatePlayer(float lastTime)
    {
        Debug.Log("RotatePlayer!!!!");
        //保存Player的旋转参数，然后旋转180度，过lastTime之后复原
        HRoguePlayerAttributeAndItemManager.Instance.GetPlayer().transform.DOLocalRotate(new Vector3(180, 0, 0), 2f, RotateMode.LocalAxisAdd);
        yield return new WaitForSeconds(lastTime);
        HRoguePlayerAttributeAndItemManager.Instance.GetPlayer().transform.DOLocalRotate(new Vector3(180, 0, 0), 2f, RotateMode.LocalAxisAdd);
    }
}