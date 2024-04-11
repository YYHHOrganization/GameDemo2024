using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Debug = UnityEngine.Debug;

public class YRouge_BattleRoom : YRouge_RoomBase
{

    // Start is called before the first frame update
    void Start()
    {
        roomType = RoomType.BattleRoom;
        base.Start();
        
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

        string enemyIDs = battleRoomData._EnemyIDField();
        
    }
    
}
