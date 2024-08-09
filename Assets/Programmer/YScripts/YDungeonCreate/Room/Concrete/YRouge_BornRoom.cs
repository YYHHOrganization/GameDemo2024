using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YRouge_BornRoom : YRouge_RoomBase
{
    public Class_BornRoomCSVFile RoomData;
    // Start is called before the first frame update
    void Start()
    {
        roomType = RoomType.BornRoom;
        base.Start();
        
        roomLittleMapMask.SetActive(false);
        //更新一下出生位置
        GameObject goBornPlace = new GameObject();
        goBornPlace.name = "RogueBornPlace";
        goBornPlace.transform.parent = transform;
        goBornPlace.transform.localPosition = new Vector3(0, 0, 0);
        
        YRogueDungeonManager.Instance.SetRogueBornPlace(goBornPlace.transform);
        
        ReadRoomData();
        GenerateOtherItems(RoomData);
    }
    void ReadRoomData()
    {
        //在房间类型中先随机选择一个房间类型，然后生成其对应的房间数据
        var RoomTypeDic = SD_BornRoomCSVFile.Class_Dic;
        if(RoomTypeDic == null)
        {
            //Debug.LogError("房间类型数据为空");
            return;
        }
        int randomIndex = Random.Range(0, RoomTypeDic.Count);

        RoomData = SD_BornRoomCSVFile.Class_Dic["6667000"+randomIndex];//66680000
    }
    private void GenerateOtherItems(Class_BornRoomCSVFile RoomCsvFile)
    {
        if(RoomCsvFile == null)
        {
            return;
        }
        string itemIDs = RoomCsvFile.OtherItemIDField;
        string[] itemIDArray = itemIDs.Split(';');
        string[] itemCounts = RoomCsvFile.OtherItemCountField.Split(';');
        
        GenerateFromItemIDArray(itemIDArray, itemCounts);
        
    }
}
