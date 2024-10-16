using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class YRouge_CreateItem 
{
    // //写一个对应，输入roomType，输出roomtype对应的类型脚本 使用字典嘛
    // Dictionary<RoomType, System.Type> roomTypeToScript = new Dictionary<RoomType, System.Type>();
    // public YRouge_CreateItem()
    // {
    //     roomTypeToScript.Add(RoomType.BattleRoom, typeof(YRouge_BattleRoom));
    //     roomTypeToScript.Add(RoomType.ItemRoom, typeof(YRouge_ItemRoom));
    //     roomTypeToScript.Add(RoomType.GameRoom, typeof(YRouge_GameRoom));
    //     roomTypeToScript.Add(RoomType.ChallengeRoom, typeof(YRouge_ChallengeRoom));
    // }

    //东西应该是从策划表读取 然后再放的
    //因为后续需要每个房间有自己的逻辑，比如进入战斗房间，门会关闭，然后打完之后门打开，因此
    //更好的方式是在每个房间有自己对应的脚本，然后在这个脚本中生成东西
    //玩家进入这个房间，然后这个房间的脚本就会被激活，然后进行相应的逻辑
    //同时这个room脚本得知道什么门是属于这个房间的，然后在这个房间的脚本中控制门的开关
    Transform parent;
    public List<YRouge_RoomBase> GenerateRoomScript(List<YRoomNode> allRoomNodes,Transform parent,Vector3Int originPosition,List<roomSpaceKeep> roomSpacesKeepList)
    {
        this.parent = parent;
        // foreach (var roomNode in allRoomNodes)
        // {
        //     GenerateSingleRoomScript(roomNode, this.parent,originPosition);
        // }
        List<YRouge_RoomBase> roomBaseLists = new List<YRouge_RoomBase>();
        for(int i = 0; i < allRoomNodes.Count; i++)
        {
            YRouge_RoomBase roomBase= GenerateSingleRoomScript(allRoomNodes[i], this.parent,originPosition,roomSpacesKeepList[i]);
            roomBaseLists.Add(roomBase);
        }
        return roomBaseLists;
    }
    YRouge_RoomBase GenerateSingleRoomScript(YRoomNode roomNode,Transform parent,Vector3Int originPosition,roomSpaceKeep roomSpacekeep)
    {
        //弄一个新的gameobject出来,然后把这个gameobject放到房间中间,挂上对应的脚本
        GameObject roomGameObject = new GameObject();
        //它的名字是房间的名字
        roomGameObject.name = roomNode.RoomType.ToString();
        roomGameObject.transform.parent = parent;
        roomGameObject.transform.position = new Vector3(
            (roomNode.BottomLeftAreaCorner.x + roomNode.TopRightAreaCorner.x) / 2, 0,
            (roomNode.BottomLeftAreaCorner.y + roomNode.TopRightAreaCorner.y) / 2) + originPosition;
        // roomGameObject.transform.localPosition = new Vector3((roomNode.BottomLeftAreaCorner.x + roomNode.TopRightAreaCorner.x) / 2, 0,
        //     (roomNode.BottomLeftAreaCorner.y + roomNode.TopRightAreaCorner.y) / 2);
        
        // if(roomTypeToScript.ContainsKey(roomNode.RoomType))
        // {
        //     roomGameObject.AddComponent(roomTypeToScript[roomNode.RoomType]);
        //     
        // }
        YRouge_RoomBase roomBase = null;
        
        if(roomNode.RoomType == RoomType.BornRoom)
        {
            roomBase = roomGameObject.AddComponent<YRouge_BornRoom>();
        }
        else if(roomNode.RoomType == RoomType.BossRoom)
        {
            roomBase = roomGameObject.AddComponent<YRouge_BossRoom>();
        }
        else if(roomNode.RoomType == RoomType.ItemRoom)
        {
            roomBase = roomGameObject.AddComponent<YRouge_ItemRoom>();
        }
        else if(roomNode.RoomType == RoomType.AdventureRoom)
        {
            roomBase = roomGameObject.AddComponent<YRouge_AdventureRoom>();
        }
        else if(roomNode.RoomType == RoomType.ShopRoom)
        {
            roomBase = roomGameObject.AddComponent<YRouge_ShopRoom>();
        }
        else if(roomNode.RoomType == RoomType.GameRoom)
        {
            roomBase = roomGameObject.AddComponent<YRouge_GameRoom>();
        }
        else if(roomNode.RoomType == RoomType.ChallengeRoom)
        {
            roomBase = roomGameObject.AddComponent<YRouge_ChallengeRoom>();
        }
        
        // else if(roomNode.RoomType == RoomType.BattleRoom)
        // {
        //     roomBase = roomGameObject.AddComponent<YRouge_BattleRoom>();
        // }
        else
        {
            //默认战斗房间 没做的先战斗吧
            roomBase = roomGameObject.AddComponent<YRouge_BattleRoom>();
        }
        
        //相互赋值
        if(roomBase != null)
            GiveRoomNodeToRoomBase(roomNode, roomBase,roomSpacekeep);

        return roomBase;
    }

    private void GiveRoomNodeToRoomBase(YRoomNode roomNode, YRouge_RoomBase roomBase,roomSpaceKeep roomSpaceKeep)
    {
        roomNode.roomScript = roomBase;
        roomBase.roomNode = roomNode;
        roomBase.SetRoomNodeSpace(roomSpaceKeep);
    }
    
}
