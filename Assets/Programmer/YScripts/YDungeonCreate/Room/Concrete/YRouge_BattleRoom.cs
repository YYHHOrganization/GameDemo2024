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
            ReadBattleRoomData();
            isFirstTimeInRoom = false;
            
            // SetAllDoorsUp();//第一次进入房间门会关
        }
        
        //生成怪物
        
    }

    void ReadBattleRoomData()
    {
        //在房间类型中先随机选择一个房间类型，然后生成其对应的房间数据
        int randomIndex = Random.Range(0, SD_BattleRoomCSVFile.Class_Dic.Count);
        Class_BattleRoomCSVFile battleRoomData = SD_BattleRoomCSVFile.Class_Dic["6662000"+randomIndex];

        //70000000;70000001
        string[] enemyIDs = battleRoomData._EnemyIDField().Split(';');
        
        
        //0.4;9.15 怪物个数/对应前面那个/0.4;9.15的意思是这种怪可能会出现从0-4的个数
        string[] enemyCounts = battleRoomData._EnemyCountField().Split(';');
        for (int i =0;i<enemyCounts.Length;i++)
        {
            string[] enemyCountRange = enemyCounts[i].Split('.');
            int minCount = int.Parse(enemyCountRange[0]);
            int maxCount = int.Parse(enemyCountRange[1]);
            int enemyCount = Random.Range(minCount, maxCount);
            
            for (int j = 0; j < enemyCount; j++)
            {
                //一只只生成这个怪
                string enemyID = enemyIDs[i];
                string EnemyAddressLink = SD_RogueEnemyCSVFile.Class_Dic[enemyID].addressableLink;
                GameObject enemy = Instantiate(Resources.Load<GameObject>(EnemyAddressLink));
                enemy.transform.parent = EnemyParent.transform;
                enemy.transform.position = transform.position + new Vector3(Random.Range(-7, 7), 0, Random.Range(-7, 7));
                enemies.Add(enemy);
                
                //生成怪物
                // HRougeAttributeManager.Instance.GenerateEnemy(enemyIDs[randomEnemyIndex], transform);
            }
        }
        
        
    }
    
}
