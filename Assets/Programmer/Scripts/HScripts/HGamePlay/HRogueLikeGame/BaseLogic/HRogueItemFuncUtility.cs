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
    
    //每次进入新房间触发的效果
    private Dictionary<string, string> enterNewRoomEffects = new Dictionary<string, string>();
    
    //每隔多少个房间触发一次的效果，这个是总的计数器
    private Dictionary<string, int> enterNewRoomEffectsRoomCounter = new Dictionary<string, int>();  //每过多少个房间触发一次
    private Dictionary<string, int> enterNewRoomEffectsRoomActualCount = new Dictionary<string, int>(); //实际的计数器
    private List<string> enterNewRoomFuncIntervalNames = new List<string>();
    
    private Dictionary<string, string> enterNewRoomEffectsInterval = new Dictionary<string, string>();
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
        int bulletDamage = int.Parse(paramList[2]);
        if (operation == "Replace")
        {
            HRoguePlayerAttributeAndItemManager.Instance.ReplaceCurBulletType(bulletType);
        }
        else if (operation == "Add")
        {
            HRoguePlayerAttributeAndItemManager.Instance.AddBulletType(bulletType, bulletDamage);
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
    
    
    # region 注册之后每个房间都调用的逻辑走这里，这里不走计数器的逻辑
    
    private bool firstRegistEnterNewRoomFunc = true;
    private bool firstRegistEnterNewRoomPositiveFunc = true;
    private bool firstRegistEnterNewRoomWithCntFunc = true;
    
    //普通进入新房间的效果，不需要房间计数器，每进入房间直接回调的函数
    public void RegisterEnterNewRoomFunc(string funcParams)  
    {
        string registerFunc = funcParams.Split('!')[0];
        string Funcparams = funcParams.Split('!')[1];
        if (firstRegistEnterNewRoomFunc)
        {
            YTriggerEvents.OnEnterRoomType += EnterNewRoomEffect;
            firstRegistEnterNewRoomFunc = false;
        }
        
        if(enterNewRoomEffects.ContainsKey(registerFunc)) return;
        enterNewRoomEffects.Add(registerFunc, Funcparams);
    }

    public void RegisterEnterNewRoomFuncWithRoomCount(string funcParams)
    {
        int roomCount = int.Parse(funcParams.Split('!')[0]);
        string registerFunc = funcParams.Split('!')[1];
        string Funcparams = funcParams.Split('!')[2];
        if (firstRegistEnterNewRoomWithCntFunc)
        {
            YTriggerEvents.OnEnterRoomType += EnterNewRoomEffectWithCnt;
            firstRegistEnterNewRoomWithCntFunc = false;
        }
        
        if(enterNewRoomEffectsInterval.ContainsKey(registerFunc)) return;
        enterNewRoomEffectsInterval.Add(registerFunc, Funcparams);
        enterNewRoomEffectsRoomCounter.Add(registerFunc, roomCount);
        enterNewRoomEffectsRoomActualCount.Add(registerFunc, 0);
        enterNewRoomFuncIntervalNames.Add(registerFunc);
    }
    
    private void EnterNewRoomEffect(object sender,YTriggerEnterRoomTypeEventArgs e)
    {
        foreach (var effect in enterNewRoomEffects)
        {
            System.Reflection.MethodInfo method = this.GetType().GetMethod(effect.Key);
            method.Invoke(this, new object[] {effect.Value});
        }
    }
    
    private void EnterNewRoomEffectWithCnt(object sender,YTriggerEnterRoomTypeEventArgs e)
    {
        for (int i = 0; i < enterNewRoomFuncIntervalNames.Count; i++)
        {
            enterNewRoomEffectsRoomActualCount[enterNewRoomFuncIntervalNames[i]]++;
            if(enterNewRoomEffectsRoomActualCount[enterNewRoomFuncIntervalNames[i]] >= enterNewRoomEffectsRoomCounter[enterNewRoomFuncIntervalNames[i]])
            {
                enterNewRoomEffectsRoomActualCount[enterNewRoomFuncIntervalNames[i]] = 0;
                System.Reflection.MethodInfo method = this.GetType().GetMethod(enterNewRoomFuncIntervalNames[i]);
                method.Invoke(this, new object[] {enterNewRoomEffectsInterval[enterNewRoomFuncIntervalNames[i]]});
            }
        }
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
    
    public void SetCameraPostProcessingEffect(string funcParams)
    {
        int randomNumber = Random.Range(0, 100);
        if (randomNumber <= 80) return;
        string[] paramList = funcParams.Split(';');
        string funcName = paramList[0];
        float lastTime = float.Parse(paramList[1]);
        HPostProcessingFilters.Instance.SetPostProcessingWithNameAndTime(funcName,lastTime);
    }
    
    public void MayKillEnemy(string funcParams)
    {
        var roomBaseScript = YRogue_RoomAndItemManager.Instance.currentRoom.GetComponent<YRouge_RoomBase>();
        if (roomBaseScript.RoomType != RoomType.BattleRoom) return;
        
        int killCnt = int.Parse(funcParams);
        DOVirtual.DelayedCall(0.5f, () =>
        {
            List<GameObject> enemies = roomBaseScript.Enemies;
            int randomNum = Random.Range(0, 100);
            if (randomNum <= 30) return;
            if (enemies!=null && enemies.Count > 0)  //当前是战斗房
            {
                for (int i = 0; i < killCnt; i++)
                {
                    int index = Random.Range(0, enemies.Count);
                    var enemy = enemies[index];
                    while (!enemy)
                    {
                        index = Random.Range(0, enemies.Count);
                        enemy = enemies[index];
                    }
                    //SetEnemyFrozen
                    if (enemy.GetComponent<HRogueEnemyPatrolAI>())
                    {
                        enemy.GetComponent<HRogueEnemyPatrolAI>().ChangeHealth(-1000);
                    }
                    else if (enemy.GetComponent<YPatrolAI>())
                    {
                        enemy.GetComponent<YPatrolAI>().die();
                    }
                }
            }
            HRogueCameraManager.Instance.ShakeCamera(10f, 0.1f);
        });
    }

    #endregion
    
    
    # region 主动道具释放的相关逻辑在这里——每隔多少房间充能这种

    private bool isPositiveItemWithTimeCountUse = false;
    
    public void RegisterEnterNewRoomPositiveFuncWithCounter(string funcName, string funcParams)
    {
        if (enterNewRoomPositiveItemCounter.ContainsKey(funcName)) return;
        //todo：因为现在只有一个初始道具，因此需要清除原来字典里有的内容
        if (enterNewRoomPositiveItemCounter.Count > 0)  //说明原来有主动道具
        {
            enterNewRoomPositiveItemCounter.Clear();
            positiveItemEffects.Clear();
        }
        
        enterNewRoomPositiveItemCounter.Add(funcName, 0);
        if (firstRegistEnterNewRoomPositiveFunc && HRoguePlayerAttributeAndItemManager.Instance.ScreenPositiveCheckType == ScreenPositiveItemCheckType.RoomCount)
        {
            YTriggerEvents.OnEnterRoomType += EnterNewRoomForPositiveItem;
            firstRegistEnterNewRoomPositiveFunc = false;
        }
        
        positiveItemEffects.Add(funcName, funcParams);
    }
    
    private void EnterNewRoomForPositiveItem(object sender,YTriggerEnterRoomTypeEventArgs e)
    {
        //todo：暂时只支持一个主动道具，后续可以考虑多个主动道具，这里先简单实现一点
        string funcName = HRoguePlayerAttributeAndItemManager.Instance.CurScreenPositiveFunc;
        enterNewRoomPositiveItemCounter[funcName]++;
        int cnt = enterNewRoomPositiveItemCounter[funcName];
        HRoguePlayerAttributeAndItemManager.Instance.RefleshPositiveItemUI(cnt);
        Debug.Log("EnterNewRomm!! " + funcName + "    " + enterNewRoomPositiveItemCounter[funcName]);
    }
    
    //当角色释放这个技能的时候，就会触发这个函数
    public void ReleasePositiveScreenItem()  //释放所有的屏幕上的主动道具技能
    {
        foreach (var effect in positiveItemEffects)
        {
            System.Reflection.MethodInfo method = this.GetType().GetMethod(effect.Key);
            method.Invoke(this, new object[] {effect.Value});
        }
    }

    public void HuangquanWuFunc(string funcParams)
    {
        int counter = HRoguePlayerAttributeAndItemManager.Instance.CurScreenPositiveItemRoomCounter;
        int realCnt = enterNewRoomPositiveItemCounter["HuangquanWuFunc"];
        if (realCnt < counter)
        {
            return;
        }
        enterNewRoomPositiveItemCounter["HuangquanWuFunc"] = 0;
        float lastTime = float.Parse(funcParams);
        HPostProcessingFilters.Instance.SetPostProcessingWithNameAndTime("HeibaiHong",lastTime);
        SetAttributeWithCertainLogic("AddAll;1");
        DOVirtual.DelayedCall(lastTime, () => 
        {
            SetAttributeWithCertainLogic("AddAll;-1");
        });

    }

    public void FrozenRoomEnemy(string funcParams)
    {
        Debug.Log("FrozenRomm!!!!");
        int counter = HRoguePlayerAttributeAndItemManager.Instance.CurScreenPositiveItemRoomCounter;
        int realCnt = enterNewRoomPositiveItemCounter["FrozenRoomEnemy"];
        if (realCnt < counter)
        {
            return;
        }
        enterNewRoomPositiveItemCounter["FrozenRoomEnemy"] = 0;
        HRoguePlayerAttributeAndItemManager.Instance.RefleshPositiveItemUI(0);
        float frozenTime = float.Parse(funcParams);
        List<GameObject> enemies =
            YRogue_RoomAndItemManager.Instance.currentRoom.GetComponent<YRouge_RoomBase>().Enemies;
        if (enemies!=null && enemies.Count > 0)  // 当前是战斗房
        {
            foreach (var enemy in enemies)
            {
                //SetEnemyFrozen
                if (enemy.GetComponent<HRogueEnemyPatrolAI>())
                {
                    enemy.GetComponent<HRogueEnemyPatrolAI>().SetFrozen(frozenTime);
                }
            }
        }
    }

    public void HurtEveryEnemyInRoom(string funcParams)
    {
        int counter = HRoguePlayerAttributeAndItemManager.Instance.CurScreenPositiveItemRoomCounter;
        int realCnt = enterNewRoomPositiveItemCounter["HurtEveryEnemyInRoom"];
        if (realCnt < counter)
        {
            return;
        }
        int hurtValue = int.Parse(funcParams);
        enterNewRoomPositiveItemCounter["HurtEveryEnemyInRoom"] = 0;
        HRoguePlayerAttributeAndItemManager.Instance.RefleshPositiveItemUI(0);
        List<GameObject> enemies =
            YRogue_RoomAndItemManager.Instance.currentRoom.GetComponent<YRouge_RoomBase>().Enemies;
        if (enemies!=null && enemies.Count > 0)  //当前是战斗房
        {
            foreach (var enemy in enemies)
            {
                //SetEnemyFrozen
                if (enemy.GetComponent<HRogueEnemyPatrolAI>())
                {
                    enemy.GetComponent<HRogueEnemyPatrolAI>().ChangeHealth(hurtValue);
                }
                else if (enemy.GetComponent<YPatrolAI>())
                {
                    enemy.GetComponent<YPatrolAI>().die();
                }
            }
        }
        HRogueCameraManager.Instance.ShakeCamera(10f, 0.1f, 0.2f);
        
    }

    public void HeiyuanbaihuaEffect(string funcParams)
    {
        int heiyuanbaihuaUseCnt = enterNewRoomPositiveItemCounter["HeiyuanbaihuaEffect"];
        int counter = HRoguePlayerAttributeAndItemManager.Instance.CurScreenPositiveItemRoomCounter;
        Debug.Log("Heiyuanbaihua!! " + heiyuanbaihuaUseCnt);
        if (heiyuanbaihuaUseCnt < counter)
        {
            return;
        }
        else
        {
            enterNewRoomPositiveItemCounter["HeiyuanbaihuaEffect"] = 0;
            HRoguePlayerAttributeAndItemManager.Instance.RefleshPositiveItemUI(0);
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
    
    #endregion
    
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

    public void Feichangjiandan(string funcParams)
    {
        string type = funcParams;
        if (type == "1")
        {
            int curHealth =
                (int)HRoguePlayerAttributeAndItemManager.Instance.characterValueAttributes["RogueCharacterHealth"];
            int healthDelta = curHealth - 2;
            HRoguePlayerAttributeAndItemManager.Instance.AddHeartOrShield("Heart", -healthDelta);
            AddAttributeValue("RogueBulletDamage;"+healthDelta);
        }
        else if (type == "2")
        {
            int curHealthUpperBound =
                (int)HRoguePlayerAttributeAndItemManager.Instance.characterValueAttributes[
                    "RogueCharacterHealthUpperBound"];
            int healthDelta = curHealthUpperBound - 2;
            HRoguePlayerAttributeAndItemManager.Instance.AddHeartOrShield("HeartUpperBound", -healthDelta);
            AddAttributeValue("RogueBulletDamage;"+(healthDelta * 3));
        }
    }

    public void Quchixingxi(string funcParams)
    {
        switch (funcParams)
        {
            case "1":
                int xinYongdianCnt = HItemCounter.Instance.CheckCountWithItemId("20000013");
                int addDamage = xinYongdianCnt / 100000;
                HItemCounter.Instance.RemoveItem("20000013", xinYongdianCnt);
                for (int i = 0; i < addDamage; i++)
                {
                    SetAttributeWithCertainLogic("Random;1");
                }
                break;
            case "2":
                int xingQiongCnt = HItemCounter.Instance.CheckCountWithItemId("20000012");
                int addDamage2 = xingQiongCnt / 500;
                HItemCounter.Instance.RemoveItem("20000012", xingQiongCnt);
                for (int i = 0; i < addDamage2; i++)
                {
                    SetAttributeWithCertainLogic("Random;1");
                }
                break;
        }
    }

    public void AddOrMultiplySth(string funcParams)
    {
        string[] args = funcParams.Split(';');
        //NotRouge;Multiply;20000008;2
        string type = args[0];
        string operation = args[1];
        string itemId = args[2];
        int value = int.Parse(args[3]);
        if (operation == "Multiply")
        {
            if (type == "NotRogue")
            {
                int cnt = HItemCounter.Instance.CheckCountWithItemId(itemId);
                if (cnt != 0)
                {
                    HItemCounter.Instance.AddItem(itemId, cnt * (value-1));
                }
            }
        }
    }

    public void SetSelfSize(string funcParams)
    {
        float value = float.Parse(funcParams);
        float size = HRoguePlayerAttributeAndItemManager.Instance.GetPlayer().transform.localScale.x;
        //clamp size * value to 0.25 ~ 1.5
        float result = value * size;
        result = Mathf.Clamp(result, 0.25f, 1.5f);
        HRoguePlayerAttributeAndItemManager.Instance.GetPlayer().transform.localScale = new Vector3(result, result, result);
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
    
    
    public IEnumerator RotatePlayer(float lastTime)
    {
        Debug.Log("RotatePlayer!!!!");
        //保存Player的旋转参数，然后旋转180度，过lastTime之后复原
        HRoguePlayerAttributeAndItemManager.Instance.GetPlayer().transform.DOLocalRotate(new Vector3(180, 0, 0), 2f, RotateMode.LocalAxisAdd);
        yield return new WaitForSeconds(lastTime);
        HRoguePlayerAttributeAndItemManager.Instance.GetPlayer().transform.DOLocalRotate(new Vector3(180, 0, 0), 2f, RotateMode.LocalAxisAdd);
    }
}
