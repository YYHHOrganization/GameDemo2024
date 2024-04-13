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
    public override void SetResultOn()
    {
        base.SetResultOn();
        
        //如果是第一次近这个房间就读取配置表，然后生成物品
        //如果不是第一次进这个房间就不生成怪物
        if (isFirstTimeInRoom)
        {
            //曾将没有转表工具时是用的以下方法做的
            // ReadRoomItem();
            // GenerateRoomItem();
            
            //生成怪物
            ReadBattleRoomData();
            isFirstTimeInRoom = false;
            SetAllDoorsUp();
            
            //然后应该去监听这个房间的怪是不是全死了
            //如果全死了就把门打开
            AddListenerOfEnemy();
            
            // SetAllDoorsUp();//第一次进入房间门会关
        }
        
        
        
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
    }


    void ReadBattleRoomData()
    {
        //在房间类型中先随机选择一个房间类型，然后生成其对应的房间数据
        
        int randomIndex = Random.Range(0, SD_BattleRoomCSVFile.Class_Dic.Count);
        
        //test:全是蜘蛛
        // randomIndex = 3;//test!!!后面记得关掉
        
        Class_BattleRoomCSVFile battleRoomData = SD_BattleRoomCSVFile.Class_Dic["6662000"+randomIndex];

        //70000000;70000001
        string[] enemyIDs = battleRoomData._EnemyIDField().Split(';');
        
        
        //0.4;9.15 怪物个数/对应前面那个/0.4;9.15的意思是这种怪可能会出现从0-4的个数
        string[] enemyCounts = battleRoomData._EnemyCountField().Split(';');
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
