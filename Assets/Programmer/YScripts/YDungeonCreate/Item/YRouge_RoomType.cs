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
        int bornRoomCount = SD_RogueRoomFile.Class_Dic["66600007"]._RoomCount();
        int shopCount = SD_RogueRoomFile.Class_Dic["66600006"]._RoomCount();
        int bossCount = SD_RogueRoomFile.Class_Dic["66600004"]._RoomCount();
        int challengeCount =SD_RogueRoomFile.Class_Dic["66600001"]._RoomCount();
        int gameCount = SD_RogueRoomFile.Class_Dic["66600003"]._RoomCount();
        int itemRoomCount =SD_RogueRoomFile.Class_Dic["66600005"]._RoomCount();
        int battleRoomCount = SD_RogueRoomFile.Class_Dic["66600000"]._RoomCount();
        int ElseRoomCount = roomCount -bornRoomCount - shopCount - battleRoomCount
                              - bossCount - challengeCount - gameCount - itemRoomCount;
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

        for (int i = 0; i < battleRoomCount; i++)
        {
            roomTypeList.Add(RoomType.BattleRoom);
        }
        
        for (int i = 0; i < ElseRoomCount; i++)
        {
            //普通战斗房间 现在作为测试，先都是道具房间
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
