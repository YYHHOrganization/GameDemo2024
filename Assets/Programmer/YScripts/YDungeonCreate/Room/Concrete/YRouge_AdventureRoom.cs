using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class YRouge_AdventureRoom : YRouge_RoomBase
{
    public Class_AdventureRoomCSVFile adventureRoomData;
    // Start is called before the first frame update
    void Start()
    {
        roomType = RoomType.AdventureRoom;
        base.Start();
        ReadBattleRoomData();
        GenerateOtherItems(adventureRoomData);
    }

    public override void EnterRoom()
    {
        base.EnterRoom();
        YTriggerEvents.OnCompleteRoom += RoomWin;
    }
    public override void ExitRoom()
    {
        base.ExitRoom();
        YTriggerEvents.OnCompleteRoom -= RoomWin;
    }

    void ReadBattleRoomData()
    {
        //在房间类型中先随机选择一个房间类型，然后生成其对应的房间数据
        
        // int randomIndex = Random.Range(0, SD_BattleRoomCSVFile.Class_Dic.Count);
        int randomIndex = Random.Range(0,SD_AdventureRoomCSVFile.Class_Dic.Count);
        
        //test:全是蜘蛛
        // randomIndex = 3;//test!!!后面记得关掉
        adventureRoomData = SD_AdventureRoomCSVFile.Class_Dic["6668000"+randomIndex];//66680000
    }

    public void RoomWin(object sender, YTriggerCountEventArgs e)
    {
        int count = e.count;
        //出现宝箱,或者掉落道具等等
        Vector3 treasurePos = transform.position;
        string chestID;
        //根据awaardGrade生成奖励
        if (count == 0)
        {
            return;
        }
        else if (count == 1||count == 2)
        {
            chestID = "10000013";
        }
        else if (count == 3||count == 4)
        {
            chestID = "10000011";
        }
        else
        {
            chestID = "10000012";
        }
        
        HOpenWorldTreasureManager.Instance.InstantiateATreasureAndSetInfoWithTypeId(chestID, treasurePos, transform);
        
        
    }
    
    
    private void GenerateOtherItems(Class_AdventureRoomCSVFile adventureRoomCsvFile)
    {
        string itemIDs = adventureRoomCsvFile.OtherItemIDField;
        string[] itemIDArray = itemIDs.Split(';');
        string[] itemCounts = adventureRoomCsvFile.OtherItemCountField.Split(';');
        
        GenerateFromItemIDArray(itemIDArray, itemCounts);
        
    }
  
}
