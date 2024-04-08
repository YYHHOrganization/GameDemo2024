using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class YRoomNode : YRouge_Node
{
    RoomType roomType;
    //脚本
    public YRouge_RoomBase roomScript;
    
    public YRoomNode(Vector2Int bottomLeftAreaCorner, Vector2Int topRightAreaCorner, YRouge_Node parentNode,int index) : base(parentNode)
    {
        BottomLeftAreaCorner = bottomLeftAreaCorner;
        TopRightAreaCorner = topRightAreaCorner;
        BottomRightAreaCorner = new Vector2Int(TopRightAreaCorner.x, BottomLeftAreaCorner.y);
        TopLeftAreaCorner = new Vector2Int(BottomLeftAreaCorner.x, TopRightAreaCorner.y);
        TreeLayerIndex = index;
        
    }
    public int RoomWidth
    {
        get { return (int)(TopRightAreaCorner.x - BottomLeftAreaCorner.x); }
    }
    public int RoomLength
    {
        get { return (int)(TopRightAreaCorner.y - BottomLeftAreaCorner.y); }
    }
    
    //房间类型
    public RoomType RoomType
    {
        get => roomType;
        set => roomType = value;
    }
   
}
