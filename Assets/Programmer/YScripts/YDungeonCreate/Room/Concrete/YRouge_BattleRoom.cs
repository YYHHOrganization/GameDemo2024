using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Debug = UnityEngine.Debug;

public class YRouge_BattleRoom : YRouge_RoomBase
{
    List<GameObject> enemies = new List<GameObject>();
    GameObject EnemyParent;
    Class_BattleRoomCSVFile battleRoomData;
    // Start is called before the first frame update
    void Start()
    {
        roomType = RoomType.BattleRoom;
        base.Start();
        
        EnemyParent = new GameObject();
        EnemyParent.transform.parent = transform;
        EnemyParent.name = "EnemyParent";
    }
    bool isFirstTimeInRoom = true;
    public override void EnterRoom()
    {
        base.EnterRoom();
    }

    protected override void FirstEnterRoom()
    {
        base.FirstEnterRoom();
        ReadBattleRoomData();
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
        HOpenWorldTreasureManager.Instance.InstantiateATreasureAndSetInfoWithTypeId("10000011", transform.position, transform);
        
        //按照概率生成道具
        GenerateRoomItemDaojuNew();
    }

    private void GenerateRoomItemDaojuNew()
    {
        string[] DropItemIDs = battleRoomData._DropItemIDField().Split(':'); 
        string[] DropItemProbabilitys = battleRoomData._DropItemIDProbabilityField().Split(':');
        for (int i = 0; i < DropItemIDs.Length; i++)
        {
            float probability = float.Parse(DropItemProbabilitys[i]);
            if (Random.Range(0, 1f) < probability)
            {
                string DropItemID = DropItemIDs[i];
                Vector3 position = new Vector3(Random.Range(-7, 7), 0.3f, Random.Range(-7, 7));
                GameObject itemRandom;
                if (DropItemID == "all")
                {
                    itemRandom = HRoguePlayerAttributeAndItemManager.Instance.RollingARandomItem(transform, position);
                }
                else
                {
                    itemRandom = HRoguePlayerAttributeAndItemManager.Instance.GiveOutAnFixedItem(DropItemID, transform,position);
                }
                itemRandom.GetComponent<HRogueItemBase>().SetBillboardEffect();
            }
        }
        
    }

    Class_BattleRoomCSVFile GetRoomIDFromDifficulty()
    {
        //根据当前在第几层第几关来决定房间的难度
        int curLevelID = YRogueDungeonManager.Instance.GetRogueLevel();
        int RoomCount = SD_BattleRoomCSVFile.Class_Dic.Count;
        
        int difficultyBias = 1;
        //根据当前关卡来决定房间的难度
        int rollTimes = 10;
        Class_BattleRoomCSVFile battleRoomData = null;//最后返回的房间数据,如果没有找到就返回最后一次的
        for (int i = 0; i < rollTimes; i++)
        {
            int randomIndex = Random.Range(0, RoomCount);
            string roomID = "6662000"+randomIndex;
            battleRoomData = SD_BattleRoomCSVFile.Class_Dic[roomID];
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
        
        //test:全是蜘蛛
        // randomIndex = 3;//test!!!后面记得关掉
        battleRoomData = GetRoomIDFromDifficulty();
        
        //70000000;70000001
        string[] enemyIDs = battleRoomData._EnemyIDField().Split(';');
        
        //0.4;9.15 怪物个数/对应前面那个/0.4;9.15的意思是这种怪可能会出现从0-4的个数
        string[] enemyCounts = battleRoomData._EnemyCountField().Split(';');
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

   
    
}
