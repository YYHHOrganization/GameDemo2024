using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class YRouge_BossRoom : YRouge_RoomBase
{
    
    GameObject EnemyParent;
    public Class_BossRoomCSVFile bossRoomData;
    // Start is called before the first frame update
    void Start()
    {
        roomType = RoomType.BossRoom;
        base.Start();
        
        EnemyParent = new GameObject();
        EnemyParent.transform.parent = transform;
        EnemyParent.name = "EnemyParent";
        
        ReadBattleRoomData();
        //生成Boss房的特殊门
        GenerateBossSpecialDoor();
        
    }

    

    bool isFirstTimeInRoom = true;
    public override void EnterRoom()
    {
        base.EnterRoom();
        
        // //如果是第一次近这个房间就读取配置表，然后生成物品
        // //如果不是第一次进这个房间就不生成怪物
        // if (isFirstTimeInRoom)
        // {
        //     //曾将没有转表工具时是用的以下方法做的
        //     // ReadRoomItem();
        //     // GenerateRoomItem();
        //     
        //     //生成怪物
        //     
        //     GenerateEmemies(bossRoomData);
        //     // GenerateOtherItems(bossRoomData);
        //     
        //     isFirstTimeInRoom = false;
        //     SetAllDoorsUp();
        //     
        //     //然后应该去监听这个房间的怪是不是全死了
        //     //如果全死了就把门打开
        //     AddListenerOfEnemy();
        //     
        //     // SetAllDoorsUp();//第一次进入房间门会关
        // }
    }
    protected override void FirstEnterRoom()
    {
        base.FirstEnterRoom();
        GenerateEmemies(bossRoomData);
        SetAllDoorsUp();
        //然后应该去监听这个房间的怪是不是全死了
        //如果全死了就把门打开
        AddListenerOfEnemy();
    }


    int dieEnemyCount = 0;
    private void AddListenerOfEnemy()
    {
        dieEnemyCount = 0;
        foreach (var enemy in enemies)
        {
            YPatrolAI patrolAI = enemy.GetComponent<YPatrolAI>();
            if (patrolAI != null)
            {
                patrolAI.OnDie += OnEnemyDie;
            }
            else
            {
                HRogueEnemyPatrolAI hRogueEnemyPatrolAI = enemy.GetComponent<HRogueEnemyPatrolAI>();
                hRogueEnemyPatrolAI.OnDie += OnEnemyDie;
            }
            
        }
    }
    
    private void OnEnemyDie()
    {
        dieEnemyCount++;
        if (dieEnemyCount == enemies.Count)
        {
            RoomWin();
        }
    }

    private void RoomWin()
    {
        //监听关闭?但是这个房间的怪都死了 怪都被销毁了
        
        SetAllDoorsDown();//门打开
        //出现宝箱,或者掉落道具等等
        Vector3 treasurePos = transform.position + new Vector3(-2, 0, -2);
        HOpenWorldTreasureManager.Instance.InstantiateATreasureAndSetInfoWithTypeId("10000012", treasurePos, transform);
        // boss 房 出现传送门
        //生成传送门
        //应该是一开始把所有都读进来，然后需要的时候再生成，比如有的是一开始就生成，有的是打完再生成，有的是进房间就生成等等
        GenerateOtherItems(bossRoomData);
        GenerateRoomItemDaojuNew();
    }
    private void GenerateRoomItemDaojuNew()
    {
        string[] DropItemIDs = bossRoomData._DropItemIDField().Split(':'); 
        string[] DropItemProbabilitys = bossRoomData._DropItemIDProbabilityField().Split(':');
        for (int i = 0; i < DropItemIDs.Length; i++)
        {
            float probability = float.Parse(DropItemProbabilitys[i]);
            if (Random.Range(0, 1f) < probability)
            {
                string DropItemID = DropItemIDs[i];
                Vector3 position = new Vector3(Random.Range(-7, 7), 0.3f, Random.Range(-7, 7));
                if (DropItemID == "all")
                {
                    GameObject lumine = HRoguePlayerAttributeAndItemManager.Instance.RollingARandomItem(transform, position);
                    lumine.GetComponent<HRogueItemBase>().SetBillboardEffect();
                }
                else
                {
                    GameObject lumine = HRoguePlayerAttributeAndItemManager.Instance.GiveOutAnFixedItem(DropItemID, transform,position);
                    lumine.GetComponent<HRogueItemBase>().SetBillboardEffect();
                }
            }
        }
        
    }

    
    
    Class_BossRoomCSVFile GetRoomIDFromDifficulty()
    {
        //根据当前在第几层第几关来决定房间的难度
        int curLevelID = YRogueDungeonManager.Instance.GetRogueLevel();
        int RoomCount = SD_BossRoomCSVFile.Class_Dic.Count;
        
        int difficultyBias = 0;
        //根据当前关卡来决定房间的难度
        int rollTimes = 10;
        Class_BossRoomCSVFile battleRoomData = null;//最后返回的房间数据,如果没有找到就返回最后一次的
        for (int i = 0; i < rollTimes; i++)
        {
            int randomIndex = Random.Range(0, RoomCount);
            string roomID = "6662100"+randomIndex;
            battleRoomData = SD_BossRoomCSVFile.Class_Dic[roomID];
            int difficulty = battleRoomData._RoomDifficultyLevel();
            
            //如果这个房间的难度 在当前关卡的难度的-difficultyBias+difficultyBias之间，那么就返回这个房间的ID
            if (difficulty >= curLevelID - difficultyBias && difficulty <= curLevelID + difficultyBias)
            {
                return battleRoomData;
            }
        }
        return battleRoomData;
    }

    void ReadBattleRoomData()
    {
        //在房间类型中先随机选择一个房间类型，然后生成其对应的房间数据
        bossRoomData = GetRoomIDFromDifficulty();
        
        // // int randomIndex = Random.Range(0, SD_BattleRoomCSVFile.Class_Dic.Count);
        // int randomIndex = Random.Range(0,SD_BossRoomCSVFile.Class_Dic.Count);
        //
        // //test:全是蜘蛛
        // // randomIndex = 3;//test!!!后面记得关掉
        // bossRoomData = SD_BossRoomCSVFile.Class_Dic["6662100"+randomIndex];
    }
    private void GenerateEmemies(Class_BossRoomCSVFile classBossRoomCsvFile)
    {
        //70000000;70000001
        string[] enemyIDs = bossRoomData._EnemyIDField().Split(';');
        
        //0.4;9.15 怪物个数/对应前面那个/0.4;9.15的意思是这种怪可能会出现从0-4的个数
        string[] enemyCounts = bossRoomData._EnemyCountField().Split(';');
        GenerateEnemyByData(enemyIDs, enemyCounts);
    }
    private void GenerateEnemyByData(string[] enemyIDs, string[] enemyCounts)
    {
        for (int i =0;i<enemyCounts.Length;i++)
        {
            string[] enemyCountRange = enemyCounts[i].Split(':');
            int minCount = int.Parse(enemyCountRange[0]);
            int maxCount = int.Parse(enemyCountRange[1]);
            int enemyCount = Random.Range(minCount, maxCount);
            
            for (int j = 0; j < enemyCount; j++)
            {
                //一只只生成这个怪
                string enemyID = enemyIDs[i];
                string EnemyAddressLink = SD_RogueEnemyCSVFile.Class_Dic[enemyID].addressableLink;
                GameObject enemy = Addressables.InstantiateAsync(EnemyAddressLink, transform).WaitForCompletion();
                enemy.transform.parent = EnemyParent.transform;
                enemy.transform.position = transform.position + new Vector3(Random.Range(-7, 7), 0, Random.Range(-7, 7));
                enemies.Add(enemy);
                
                //生成怪物
                // HRougeAttributeManager.Instance.GenerateEnemy(enemyIDs[randomEnemyIndex], transform);
            }
        }
    }


    private void GenerateOtherItems(Class_BossRoomCSVFile bossRoomData)
    {
        string itemIDs = bossRoomData.OtherItemIDField;
        string[] itemIDArray = itemIDs.Split(';');
        string[] itemCounts = bossRoomData.OtherItemCountField.Split(';');
        GenerateFromItemIDArray(itemIDArray, itemCounts);
        // for (int i = 0; i < itemIDArray.Length; i++)
        // {
        //     string[] itemCountRange = itemCounts[i].Split(':');
        //     int minCount = int.Parse(itemCountRange[0]);
        //     int maxCount = int.Parse(itemCountRange[1]);
        //     int itemCount = Random.Range(minCount, maxCount);
        //     
        //     for(int j = 0; j < itemCount; j++)
        //     {
        //         string itemID = itemIDArray[i];
        //         Class_RogueCommonItemCSVFile itemData = SD_RogueCommonItemCSVFile.Class_Dic[itemID];
        //         string itemAddressLink =itemData.addressableLink;
        //         GameObject item = Addressables.InstantiateAsync(itemAddressLink, transform).WaitForCompletion();
        //         item.transform.parent = transform;
        //         item.transform.position = transform.position;
        //         if(itemData.GeneratePlace == "middle")
        //         {
        //             item.transform.position = transform.position;
        //         }
        //         else if (itemData.GeneratePlace == "random")
        //         {
        //             item.transform.position = transform.position + new Vector3(Random.Range(-7, 7), 0, Random.Range(-7, 7));
        //         }
        //     }
        //     
        // }
        
    }
    private void GenerateBossSpecialDoor()
    {
        string AddLink = "YHorizontalDoorBoss";
        //在所有门的位置生成特殊门 public List<GameObject> doors=new List<GameObject>();
        foreach (var door in horizontaldoors)
        {
            
            GameObject doorBoss = Addressables.InstantiateAsync(AddLink, transform).WaitForCompletion();
            doorBoss.transform.position = new Vector3( door.transform.position.x, 0, door.transform.position.z);
            doorBoss.transform.rotation = door.transform.rotation;
            doorBoss.transform.parent = transform;
            
        }
        
        AddLink = "YVertiacalDoorBoss";
        foreach (var door in vertiacaldoors)
        {
            GameObject doorBoss = Addressables.InstantiateAsync(AddLink, transform).WaitForCompletion();
            doorBoss.transform.position = new Vector3( door.transform.position.x, 0, door.transform.position.z);
            doorBoss.transform.rotation = door.transform.rotation;
            doorBoss.transform.parent = transform;
            
        }
        
        //GameObject door = Addressables.InstantiateAsync(AddLink, transform).WaitForCompletion();
    }
}
