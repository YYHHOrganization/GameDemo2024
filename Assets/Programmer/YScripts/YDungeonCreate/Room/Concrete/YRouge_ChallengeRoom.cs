using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YRouge_ChallengeRoom : YRouge_RoomBase
{
    public Class_ChallengeRoomCSVFile challengeRoomData;
    // Start is called before the first frame update
    void Start()
    {
        roomType = RoomType.ChallengeRoom;
        base.Start();
        
        ReadBattleRoomData();
        GenerateOtherItems(challengeRoomData);
        
        //测试生成一些物品
        //GenerateItemPlacement();
    }
    

    void ReadBattleRoomData()
    {
        //在房间类型中先随机选择一个房间类型，然后生成其对应的房间数据
        int randomIndex = Random.Range(0,SD_ChallengeRoomCSVFile.Class_Dic.Count);

        // randomIndex = 3;//test!!!后面记得关掉
        challengeRoomData = SD_ChallengeRoomCSVFile.Class_Dic["6661000"+randomIndex];//66680000
    }

    private void GenerateOtherItems(Class_ChallengeRoomCSVFile challengeRoomDataCsvFile)
    {
        string itemIDs = challengeRoomDataCsvFile.OtherItemIDField;
        string[] itemIDArray = itemIDs.Split(';');
        string[] itemCounts = challengeRoomDataCsvFile.OtherItemCountField.Split(';');
        
        GenerateFromItemIDArray(itemIDArray, itemCounts);
        
    }
}
