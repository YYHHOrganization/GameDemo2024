using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class YRouge_GameRoom : YRouge_RoomBase
{
    public Class_GameRoomCSVFile gameRoomData;
    
    // Start is called before the first frame update
    void Start()
    {
        roomType = RoomType.GameRoom;
        base.Start();
     
        ReadBattleRoomData();
        GenerateOtherItems(gameRoomData);
    }

    public override void EnterRoom()
    {
        base.EnterRoom();
    }
    public override void ExitRoom()
    {
        base.ExitRoom();
    }

    void ReadBattleRoomData()
    {
        //在房间类型中先随机选择一个房间类型，然后生成其对应的房间数据
        
        // int randomIndex = Random.Range(0, SD_BattleRoomCSVFile.Class_Dic.Count);
        int randomIndex = Random.Range(0,SD_GameRoomCSVFile.Class_Dic.Count);
        
        //test:全是蜘蛛
        // randomIndex = 3;//test!!!后面记得关掉
        gameRoomData = SD_GameRoomCSVFile.Class_Dic["6663000"+randomIndex];//66680000
    }

    
    private void GenerateOtherItems(Class_GameRoomCSVFile gameRoomCsvFile)
    {
        string itemIDs = gameRoomCsvFile.OtherItemIDField;
        string[] itemIDArray = itemIDs.Split(';');
        string[] itemCounts = gameRoomCsvFile.OtherItemCountField.Split(';');
        
        GenerateFromItemIDArray(itemIDArray, itemCounts);
        
    }

}
