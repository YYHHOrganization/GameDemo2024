using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YRouge_RoomGenerator 
{
    int maxIterations;
    int roomWidthMin;
    int roomLengthMin;
    public YRouge_RoomGenerator(int maxIterations, int roomWidthMin, int roomLengthMin)
    {
        Debug.Log("Room Generator Created");
        this.maxIterations = maxIterations;
        this.roomWidthMin = roomWidthMin;
        this.roomLengthMin = roomLengthMin;
    }
    /// <summary>
    /// 在给定的空间中生成房间，房间会比空间略微小一些
    /// </summary>
    /// <param name="roomSpaces"></param>
    /// <returns></returns>
    public List<YRoomNode> GenerateRoomsInGivenSpaces(List<YRouge_Node> roomSpaces,float roomBottomCornerModifier,float roomTopCornerModifier,int offset)
    {
        List<YRoomNode> roomNodes = new List<YRoomNode>();
        foreach (YRoomNode roomSpace in roomSpaces)
        {
            Vector2Int newBottomLeftPoint =
                YRouge_StructureHelper.GenerateBottomLeftCornerBetween(
                    roomSpace.BottomLeftAreaCorner, roomSpace.TopRightAreaCorner, roomBottomCornerModifier,offset);
            Vector2Int newTopRightPoint = 
                YRouge_StructureHelper.GenerateTopRightCornerBetween(
                    roomSpace.BottomLeftAreaCorner, roomSpace.TopRightAreaCorner, roomTopCornerModifier, offset);
            
            //每个房间和其他房间的大小都有略微不同
            roomSpace.BottomLeftAreaCorner = newBottomLeftPoint;
            roomSpace.TopRightAreaCorner = newTopRightPoint;
            roomSpace.BottomRightAreaCorner = new Vector2Int(newTopRightPoint.x, newBottomLeftPoint.y);
            roomSpace.TopLeftAreaCorner = new Vector2Int(newBottomLeftPoint.x, newTopRightPoint.y);
            roomNodes.Add((YRoomNode)roomSpace);
        }
        return roomNodes;
    }
}
