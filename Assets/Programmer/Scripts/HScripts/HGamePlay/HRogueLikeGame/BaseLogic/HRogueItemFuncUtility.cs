using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
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
        // System.Reflection.MethodInfo method = this.GetType().GetMethod(funcName);
        // method.Invoke(this, new object[] {funcParams});
        // 反射太慢了，考虑最暴力的打表，直接写出所有的函数
        switch (funcName)
        {
            case "AddAttributeValue":
                AddAttributeValue(funcParams);
                break;
            case "AddHeartOrShield":
                AddHeartOrShield(funcParams);
                break;
            case "AddMoney":
                AddMoney(funcParams);
                break;
            case "SetOrAddBulletType":
                SetOrAddBulletType(funcParams);
                break;
            case "GiveARandomItemWithIdRange":
                GiveARandomItemWithIdRange(funcParams);
                break;
            case "SetEveryItemName":
                SetEveryItemName(funcParams);
                break;
            case "Bishangshuangyan":
                Bishangshuangyan(funcParams);
                break;
            case "AddEnemyHealth":
                AddEnemyHealth(funcParams);
                break;
            case "GetAllBlessWithKind":
                GetAllBlessWithKind(funcParams);
                break;
            case "SetAttributeWithCertainLogic":
                SetAttributeWithCertainLogic(funcParams);
                break;
            case "RegisterEnterNewRoomFunc":
                RegisterEnterNewRoomFunc(funcParams);
                break;
            case "RegisterEnterNewRoomFuncWithRoomCount":
                RegisterEnterNewRoomFuncWithRoomCount(funcParams);
                break;
            case "SetShopItemPriceMultiply":
                SetShopItemPriceMultiply(funcParams);
                break;
            case "SetSelfSize":
                SetSelfSize(funcParams);
                break;
            case "AddOrMultiplySth":
                AddOrMultiplySth(funcParams);
                break;
            case "Quchixingxi":
                Quchixingxi(funcParams);
                break;
            case "Feichangjiandan":
                Feichangjiandan(funcParams);
                break;
            case "GameAttributeLogic":
                GameAttributeLogic(funcParams);
                break;
            case "HideMySelf":
                HideMySelf(funcParams);
                break;
            case "GetGanhaiPicture":
                GetGanhaiPicture(funcParams);
                break;
            case "PortalToSomeRoom":
                PortalToSomeRoom(funcParams);
                break;
            case "PortalToSpecialMap":
                PortalToSpecialMap(funcParams);
                break;
            case "SummonSth":
                SummonSth(funcParams);
                break;
            default:
                System.Reflection.MethodInfo method = this.GetType().GetMethod(funcName);
                method.Invoke(this, new object[] {funcParams});
                break;
        }
    }

    private void SummonSth(string funcParams)
    {
        string[] paramList = funcParams.Split(';');
        string kind = paramList[0];
        string type = paramList[1];
        int number = int.Parse(paramList[2]);
        switch (kind)
        {
            case "Catcake":
                int catcakeCount = SD_RoguePetCSVFile.Class_Dic.Count;
                if (type == "Random")
                {
                    for (int i = 0; i < number; i++)
                    {
                        int randomIndex = Random.Range(0, catcakeCount);
                        string petId = SD_RoguePetCSVFile.Class_Dic.Keys.ToArray()[randomIndex];
                        YPlayModeController.Instance.SetCatcake(petId);
                    }
                }
                else if (type == "All")
                {
                    for (int i = 0; i < number; i++)
                    {
                        foreach (string key in SD_RoguePetCSVFile.Class_Dic.Keys)
                        {
                            YPlayModeController.Instance.SetCatcake(key);
                        }
                    }
                }
                break;
        }
    }

    public void UsePositiveItemInBag(string funcName, string funcParams)
    {
        switch (funcName)
        {
            case "AddMoney":
                AddMoney(funcParams);
                break;
            default:
                System.Reflection.MethodInfo method = this.GetType().GetMethod(funcName);
                method.Invoke(this, new object[] {funcParams});
                break;
        }
        
    }
    
    private void AddAttributeValue(string funcParams)
    {
        Debug.Log("AddAttributeValue");
        //根据funcParams的内容，将对应的属性值加上对应的数值
        string[] paramList = funcParams.Split(';');
        string attributeName = (string)paramList[0];
        float attributeValue = float.Parse(paramList[1]);
        HRoguePlayerAttributeAndItemManager.Instance.AddAttributeValue(attributeName, attributeValue);
    }

    private void GetGanhaiPicture(string funcParams)
    {
        //todo：全屏幕要放礼花效果，并且给出一条Message
        //有绀海组的全部照片，全部属性*2
        if (funcParams == "seele")
        {
            //检查是否有bronya
            if(!HItemCounter.Instance.RogueItemCounts.ContainsKey("80000101")) return;
            if (HItemCounter.Instance.RogueItemCounts["80000101"] > 0)
            {
                SetAttributeWithCertainLogic("MultiplyAll;2");
                GiveOutSomeVFXAroundPlayer("Lihua1");
                GiveOutSomeVFXAroundPlayer("Lihua2");
                HMessageShowMgr.Instance.ShowMessage("ROGUE_GANHAI_MESSAGE_1");
            }
        }
        else if(funcParams == "bronya")
        {
            if(!HItemCounter.Instance.RogueItemCounts.ContainsKey("80000100")) return;
            //检查是否有seele
            if (HItemCounter.Instance.RogueItemCounts["80000100"] > 0)
            {
                SetAttributeWithCertainLogic("MultiplyAll;2");
                GiveOutSomeVFXAroundPlayer("Lihua1");
                GiveOutSomeVFXAroundPlayer("Lihua2");
                HMessageShowMgr.Instance.ShowMessage("ROGUE_GANHAI_MESSAGE_1");
            }
        }
    }

    private void GiveOutSomeVFXAroundPlayer(string vfxLink)
    {
        //在玩家周围生成一圈效果
        GameObject vfx = Addressables.LoadAssetAsync<GameObject>(vfxLink).WaitForCompletion();
        Transform player = HRoguePlayerAttributeAndItemManager.Instance.GetPlayer().transform;
        Vector3 playerPos = player.position;
        for (int i = 0; i < 360; i+=30)
        {
            Vector3 pos = playerPos + new Vector3(Mathf.Cos(i * Mathf.Deg2Rad), 0, Mathf.Sin(i * Mathf.Deg2Rad));
            GameObject tmpVFX = Instantiate(vfx, pos, Quaternion.identity);
            Destroy(tmpVFX, 10f);
        }
    }

    private void AddHeartOrShield(string funcParams)
    {
        //根据funcParams的内容，决定加血/加护盾，或者是加血量上限/加护盾上限
        string[] paramList = funcParams.Split(';');
        string attributeName = (string)paramList[0];
        int attributeValue = int.Parse(paramList[1]);
        HRoguePlayerAttributeAndItemManager.Instance.AddHeartOrShield(attributeName, attributeValue);
    }

    private void HideMySelf(string funcParams)
    {
        float time = float.Parse(funcParams);
        HRoguePlayerAttributeAndItemManager.Instance.GetPlayer().GetComponentInChildren<SkinnedMeshRenderer>().enabled =
            false;
        DOVirtual.DelayedCall(time, () =>
        {
            HRoguePlayerAttributeAndItemManager.Instance.GetPlayer().GetComponentInChildren<SkinnedMeshRenderer>()
                .enabled = true;
        });
    }
    
    private void GameAttributeLogic(string funcParams)
    {
        switch (funcParams)
        {
            case "1": //将信用点数量与星琼数量交换
                int xingQiongCnt = HItemCounter.Instance.CheckCountWithItemId("20000012");
                int xinYongdianCnt = HItemCounter.Instance.CheckCountWithItemId("20000013");
                HItemCounter.Instance.RemoveItem("20000012", xingQiongCnt);
                HItemCounter.Instance.RemoveItem("20000013", xinYongdianCnt);
                HItemCounter.Instance.AddItem("20000012", xinYongdianCnt);
                HItemCounter.Instance.AddItem("20000013", xingQiongCnt);
                break;
            case "2":  //将角色所有属性值+1或者-1
                SetAttributeWithCertainLogic("RandomUpOrDownEveryone;1");
                break;
            case "3": //扣除角色的全部护盾值，并将其添加到血量上限上
                int shield = (int)HRoguePlayerAttributeAndItemManager.Instance.characterValueAttributes["RogueCharacterShield"];
                HRoguePlayerAttributeAndItemManager.Instance.AddHeartOrShield("HeartUpperBound", shield);
                HRoguePlayerAttributeAndItemManager.Instance.AddHeartOrShield("Shield", -shield);
                break;
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
    }
    
    private void PortalToSomeRoom(string funcParams)
    {
        // GameObject player = HRoguePlayerAttributeAndItemManager.Instance.GetPlayer();
        GameObject player = YPlayModeController.Instance.curCharacter;
        List<YRouge_RoomBase> room = YRogueDungeonManager.Instance.GetRoomBaseList();
        switch (funcParams)
        {
            case "BossRoom":
                foreach (var roomBase in room)
                {
                    if (roomBase.RoomType == RoomType.BossRoom)
                    {
                        //坑！！！！！！！！
                        player.GetComponent<CharacterController>().enabled = false;
                        Debug.Log("Teleport to BossRoom");
                        
                        player.transform.position= new Vector3(
                            roomBase.transform.position.x,
                            roomBase.transform.position.y+10f,
                            roomBase.transform.position.z);
                        player.GetComponent<CharacterController>().enabled = true;
                        break;
                    }
                }
                break;
            case "ItemRoom":
                foreach (var roomBase in room)
                {
                    if (roomBase.RoomType == RoomType.ItemRoom)
                    {
                        player.GetComponent<CharacterController>().enabled = false;
                        Debug.Log("Teleport to BossRoom");
                        player.transform.position= new Vector3(
                            roomBase.transform.position.x,
                            roomBase.transform.position.y+10f,
                            roomBase.transform.position.z);
                        player.GetComponent<CharacterController>().enabled = true;
                        break;
                    }
                }
                break;
            case "ChallengeRoom":
                foreach (var roomBase in room)
                {
                    if (roomBase.RoomType == RoomType.ChallengeRoom)
                    {
                        player.GetComponent<CharacterController>().enabled = false;
                        Debug.Log("Teleport to BossRoom");
                        player.transform.position= new Vector3(
                            roomBase.transform.position.x,
                            roomBase.transform.position.y+10f,
                            roomBase.transform.position.z);
                        player.GetComponent<CharacterController>().enabled = true;
                        break;
                    }
                }
                break;
        }
    }
    
    private void PortalToSpecialMap(string funcParams)
    {
        //todo：传送到特定的地图
        Debug.Log("PortalToSpecialMap"+funcParams);
        GameObject player = YPlayModeController.Instance.curCharacter;
        YSpecialMap specialMap = YRogueDungeonManager.Instance.GetSpecialMap(funcParams);
        //通知这个地图 角色进入了
        specialMap.OnPlayerEnter();
        
        //存储原来的位置  方便后续传送
        YRogueDungeonManager.Instance.SetTransferPlace(player.transform);
        
        //传送角色到特定地图的出生点
        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = specialMap.BornPlace.position;
        
        //在duration时间内，从bornPlace移动到landingPlace
        float duration = 3f;
        player.transform.DOMove(specialMap.LandingPlace.position, duration).OnComplete(() =>
        {
            player.GetComponent<CharacterController>().enabled = true;
        });
        // player.GetComponent<CharacterController>().enabled = true;
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
    
    private void SetOrAddBulletType(string funcParams)
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

    private void SetEveryItemName(string funcParams)
    {
        foreach (var item in yPlanningTable.Instance.rogueItemBases)
        {
            item.Value.rogueItemNameShowDefault = bool.Parse(funcParams);
        }
    }

    private void AddEnemyHealth(string funcParams)
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

    private void GetAllBlessWithKind(string funcParams)
    {
        HRoguePlayerAttributeAndItemManager.Instance.GiveOutRuanmeiItem(funcParams);
    }
    
    private void SetAttributeWithCertainLogic(string funcParams)
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
                int randomIndex = UnityEngine.Random.Range(0, attributes.Count - 1);
                characterAttributes[attributes[randomIndex]] += value;
                if(characterAttributes[attributes[randomIndex]] < 1.0f) 
                {
                    characterAttributes[attributes[randomIndex]] = 1.0f;
                }
                break;
            
            case "RandomUpOrDownEveryone":
                for (int i = 0; i < attributes.Count; i++)
                {
                    int randomValue = UnityEngine.Random.Range(0, 2);
                    if (randomValue == 0)
                    {
                        characterAttributes[attributes[i]] += value;
                    }
                    else
                    {
                        characterAttributes[attributes[i]] -= value;
                    }
                    if(characterAttributes[attributes[i]] < 1.0f) 
                    {
                        characterAttributes[attributes[i]] = 1.0f;
                    }
                }
                break;
            
            case "MultiplyAll":
                for (int i = 0; i < attributes.Count; i++)
                {
                    characterAttributes[attributes[i]] *= value;
                    if(characterAttributes[attributes[i]] < 1.0f)  //角色的各种普通属性的最小值是1
                    {
                        characterAttributes[attributes[i]] = 1.0f;
                    }
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
    private void RegisterEnterNewRoomFunc(string funcParams)  
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

    private void RegisterEnterNewRoomFuncWithRoomCount(string funcParams)
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

    public void SetCameraPostProcessingEffect2(string funcParams)  //写出这种函数是因为在注册的时候会用不同的函数名字存放在字典里，因此每次需要多写一个函数，todo：不知道怎么优化，先这样吧。
    {
        SetCameraPostProcessingEffect(funcParams);
    }
    
    public void SetCameraPostProcessingEffect3(string funcParams)
    {
        SetCameraPostProcessingEffect(funcParams);
    }

    public void MayKillEnemy2(string funcParams)
    {
        MayKillEnemy(funcParams);
    }
    
    public void MayKillEnemy(string funcParams)
    {
        var roomBaseScript = YRogue_RoomAndItemManager.Instance.currentRoom.GetComponent<YRouge_RoomBase>();
        if (roomBaseScript.RoomType != RoomType.BattleRoom) return;
        
        int checkValue = 100 - (int)(100 * float.Parse(funcParams.Split(';')[1]));
        int randomNum = Random.Range(0, 100);
        if (randomNum <= checkValue) return;
        
        List<GameObject> enemies = roomBaseScript.Enemies;
        
        if (funcParams.Split(';')[0] == "All")
        {
            DOVirtual.DelayedCall(0.5f, () =>
            {
                //杀死所有的敌人
                if (enemies != null && enemies.Count > 0) //当前是战斗房
                {
                    foreach (var enemy in enemies)
                    {
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
        else
        {
            int killCnt = int.Parse(funcParams.Split(';')[0]);
            
            DOVirtual.DelayedCall(0.5f, () =>
            {
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
    
    public void EnterNewRoomForPositiveItem(object sender,YTriggerEnterRoomTypeEventArgs e)
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

    public void HurtMySelfAndDoSomething(string funcParams)
    {
        //每次扣除自身半格血，造成3点全房间伤害
        if(HRoguePlayerAttributeAndItemManager.Instance.characterValueAttributes["RogueCharacterHealth"] > 1)
        {
            AddHeartOrShield("Health;-1");
            HurtEveryEnemyInRoomWithValue(-3);
        }
    }

    public void HurtMySelfAndDoSomething2(string funcParams)
    {
        //主动道具：扣除100点星琼，造成3点全房间伤害
        int xingQiongCnt = HItemCounter.Instance.CheckCountWithItemId("20000012");
        if (xingQiongCnt > 100)
        {
            HItemCounter.Instance.RemoveItem("20000012", 100);
            HurtEveryEnemyInRoomWithValue(-3);
        }
    }
    
    public void HurtMySelfAndDoSomething3(string funcParams)
    {
        //主动道具：扣除10000点信用点，造成3点全房间伤害
        int xinYongdianCnt = HItemCounter.Instance.CheckCountWithItemId("20000013");
        if (xinYongdianCnt > 10000)
        {
            HItemCounter.Instance.RemoveItem("20000013", 10000);
            HurtEveryEnemyInRoomWithValue(-3);
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
        HRoguePlayerAttributeAndItemManager.Instance.RefleshPositiveItemUI(0);
        float lastTime = float.Parse(funcParams);
        HPostProcessingFilters.Instance.SetPostProcessingWithNameAndTime("HeibaiHong",lastTime);
        float timeScale = Time.timeScale;
        Time.timeScale = 0.2f;
        DOVirtual.DelayedCall(1.5f, () =>
        {
            Time.timeScale = timeScale;
        });
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
                if(enemy == null) continue;
                //SetEnemyFrozen
                if (enemy.GetComponent<HRogueEnemyPatrolAI>())
                {
                    enemy.GetComponent<HRogueEnemyPatrolAI>().SetFrozen(frozenTime);
                }
            }
        }
    }

    private void HurtEveryEnemyInRoomWithValue(int value)
    {
        List<GameObject> enemies =
            YRogue_RoomAndItemManager.Instance.currentRoom.GetComponent<YRouge_RoomBase>().Enemies;
        if (enemies!=null && enemies.Count > 0)  //当前是战斗房
        {
            for(int i = 0; i < enemies.Count;i++)
            {
                if(enemies[i] == null) continue;
                //SetEnemyFrozen
                if (enemies[i].GetComponent<HRogueEnemyPatrolAI>())
                {
                    enemies[i].GetComponent<HRogueEnemyPatrolAI>().ChangeHealth(value);
                }
                else if (enemies[i].GetComponent<YPatrolAI>())
                {
                    enemies[i].GetComponent<YPatrolAI>().die();
                }
            }
        }
        HRogueCameraManager.Instance.ShakeCamera(10f, 0.1f, 0.2f);
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

    private void Feichangjiandan(string funcParams)
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

    private void Quchixingxi(string funcParams)
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
            case "3": //失去全部的信用点，每失去100000信用点，获得一颗心上限
                int xinYongdianCnt3 = HItemCounter.Instance.CheckCountWithItemId("20000013");
                int addCnt = xinYongdianCnt3 / 100000;
                HItemCounter.Instance.RemoveItem("20000013", xinYongdianCnt3);
                for (int i = 0; i < addCnt; i++)
                {
                    AddHeartOrShield("HeartUpperBound;1");
                }
                break;
            case "4": //失去全部的星琼，每失去500星琼，获得100000信用点
                int xingQiongCnt4 = HItemCounter.Instance.CheckCountWithItemId("20000012");
                int addCnt2 = xingQiongCnt4 / 500;
                HItemCounter.Instance.RemoveItem("20000012", xingQiongCnt4);
                for (int i = 0; i < addCnt2; i++)
                {
                    AddMoney("RogueXinyongdian;100000");
                }
                break;
        }
    }

    private void AddOrMultiplySth(string funcParams)
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

    private void SetSelfSize(string funcParams)
    {
        float value = float.Parse(funcParams);
        float size = HRoguePlayerAttributeAndItemManager.Instance.GetPlayer().transform.localScale.x;
        //clamp size * value to 0.25 ~ 1.5
        float result = value * size;
        result = Mathf.Clamp(result, 0.25f, 1.5f);
        HRoguePlayerAttributeAndItemManager.Instance.GetPlayer().transform.localScale = new Vector3(result, result, result);
    }
    
    private void Bishangshuangyan(string funcParams)
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

    private void GiveARandomItemWithIdRange(string funcParams)
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
    
    public IEnumerator RotatePlayer(float lastTime)
    {
        Debug.Log("RotatePlayer!!!!");
        //保存Player的旋转参数，然后旋转180度，过lastTime之后复原
        HRoguePlayerAttributeAndItemManager.Instance.GetPlayer().transform.DOLocalRotate(new Vector3(180, 0, 0), 2f, RotateMode.LocalAxisAdd);
        yield return new WaitForSeconds(lastTime);
        HRoguePlayerAttributeAndItemManager.Instance.GetPlayer().transform.DOLocalRotate(new Vector3(180, 0, 0), 2f, RotateMode.LocalAxisAdd);
    }
    
    public void CancelAllListeners()
    {
        YTriggerEvents.OnEnterRoomType -= EnterNewRoomEffect;
        YTriggerEvents.OnEnterRoomType -= EnterNewRoomEffectWithCnt;
        YTriggerEvents.OnEnterRoomType -= EnterNewRoomForPositiveItem;
    }
}
