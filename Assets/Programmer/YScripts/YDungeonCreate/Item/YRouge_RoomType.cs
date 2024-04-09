using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YRouge_RoomType
{
    
    //根据房间数量，随机给房间一个类型，其中有1-2个是商店，1个boss房间，1个挑战房，1个游戏房，1个道具房间，剩下的是普通战斗房间
    public void GenerateRoomType(List<YRoomNode> allRoomNodes)
    {
        //根据房间数量，随机给房间一个类型，其中有1-2个是商店，1个boss房间，1个挑战房，1个游戏房，1个道具房间，剩下的是普通战斗房间
        int roomCount = allRoomNodes.Count;
        int bornRoomCount = 1;
        int shopCount = Random.Range(1, 3);
        int bossCount = 1;
        int challengeCount = 1;
        int gameCount = 1;
        int itemRoomCount = 1;
        int normalRoomCount = roomCount -bornRoomCount - shopCount - bossCount - challengeCount - gameCount - itemRoomCount;
        List<RoomType> roomTypeList = new List<RoomType>();
        for (int i = 0; i < bornRoomCount; i++)
        {
            roomTypeList.Add(RoomType.BornRoom);
        }
        for (int i = 0; i < shopCount; i++)
        {
            roomTypeList.Add(RoomType.ShopRoom);
        }
        for (int i = 0; i < bossCount; i++)
        {
            roomTypeList.Add(RoomType.BossRoom);
        }
        for (int i = 0; i < challengeCount; i++)
        {
            roomTypeList.Add(RoomType.ChallengeRoom);
        }
        for (int i = 0; i < gameCount; i++)
        {
            roomTypeList.Add(RoomType.GameRoom);
        }
        for (int i = 0; i < itemRoomCount; i++)
        {
            roomTypeList.Add(RoomType.ItemRoom);
        }
        for (int i = 0; i < normalRoomCount; i++)
        {
            roomTypeList.Add(RoomType.BattleRoom);
        }
        for (int i = 0; i < allRoomNodes.Count; i++)
        {
            int randomIndex = Random.Range(0, roomTypeList.Count);
            allRoomNodes[i].RoomType = roomTypeList[randomIndex];
            
            roomTypeList.RemoveAt(randomIndex);
        }
        
    }
}
