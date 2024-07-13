using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;



public class YRogue_RoomAndItemManager : MonoBehaviour
{
    //单例
    private static YRogue_RoomAndItemManager instance;
    public static YRogue_RoomAndItemManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<YRogue_RoomAndItemManager>();
            }

            return instance;
        }
    }

    public GameObject currentRoom;
    private GameObject lastRoom;
    public GameObject LastRoom
    {
        get
        {
            return lastRoom;
        }
    }
    public void SetCurRoom(GameObject room)
    {
        lastRoom = currentRoom;
        currentRoom = room;
    }
    
    List<RoomData> roomDataList;

    //读取表对应的东西，存入RoomData
    // 读取CSV文件并解析数据
    public void ReadRoomCSVFile(string roomPath)
    {
        AssetReference _addressableTextAsset = new AssetReference(roomPath);
        _addressableTextAsset.LoadAssetAsync<TextAsset>().Completed += handle =>
        {
            //最后会有个\r\n
            string[] fileData = handle.Result.text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            string[] headers = fileData[1].Split(',');
            roomDataList = new List<RoomData>();
            for (int i = 3; i < fileData.Length; i++)
            {
                
                string[] rowData = fileData[i].Split(',');
                RoomData roomData = new RoomData();
                List<RoomItemData> roomItemDataList = new List<RoomItemData>();
                
                RoomItemData roomItemData = new RoomItemData();//目前表里只有一个值  先这么写吧
                for (int j = 0; j < headers.Length; j++)
                {
                    switch (headers[j])
                    {
                        case nameof(RoomData.roomType):
                            roomData.roomType = (RoomType)Enum.Parse(typeof(RoomType), rowData[j]);
                            break;
                        case nameof(RoomData.roomID):
                            roomData.roomID = int.Parse(rowData[j]);
                            break; 
                        case nameof(RoomItemData.FixedItemID):
                            roomItemData.FixedItemID = int.Parse(rowData[j]);
                            break;
                        case nameof(RoomItemData.FixedItemCount):
                            roomItemData.FixedItemCount = int.Parse(rowData[j]);
                            break;    
                           
                        // Add more cases as needed
                    }
                }
                roomItemDataList.Add(roomItemData);
                roomData.roomItemDataList = roomItemDataList;

                roomDataList.Add(roomData);
            }
            
            Addressables.Release(handle);
        };
    }
    
    public RoomData GetRoomDataByRoomType(RoomType roomType)
    {
        foreach (var roomData in roomDataList)
        {
            if (roomData.roomType == roomType)
            {
                return roomData;
            }
        }

        return new RoomData();
    }

    List<EnemyData> enemyDataList;
    public void ReadEnemyCSVFile(string EnemyPath)
    {
        AssetReference _addressableTextAsset = new AssetReference(EnemyPath);
        _addressableTextAsset.LoadAssetAsync<TextAsset>().Completed += handle =>
        {
            //最后会有个\r\n
            string[] fileData = handle.Result.text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            string[] headers = fileData[1].Split(',');
            enemyDataList = new List<EnemyData>();
            for (int i = 3; i < fileData.Length; i++)
            {
                string[] rowData = fileData[i].Split(',');
                EnemyData singleData = new EnemyData();

                for (int j = 0; j < headers.Length; j++)
                {
                    switch (headers[j])
                    {
                        case nameof(EnemyData.enemyID):
                            singleData.enemyID = int.Parse(rowData[j]);
                            break;
                        case nameof(EnemyData.addressableLink):
                            singleData.addressableLink = rowData[j];
                            break;
                           
                        // Add more cases as needed
                    }
                }
                enemyDataList.Add(singleData);
            }
            
            Addressables.Release(handle);
        };
    }
    public string GetRoomItemLink(int ItemID)
    {
        //根据ItemID生成物品，从enemyData表里读取
        foreach (var enemyData in enemyDataList)
        {
            if (enemyData.enemyID == ItemID)
            {
                return enemyData.addressableLink;
                
            }
        }
        return null;
    }
}
