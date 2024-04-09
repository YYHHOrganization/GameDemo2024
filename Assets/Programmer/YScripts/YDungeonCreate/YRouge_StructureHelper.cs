using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class YRouge_StructureHelper 
{
    /// <summary>
    /// 此方法用于遍历树结构，提取最底层的叶子节点
    /// </summary>
    /// <param name="parentNode"></param>
    /// <returns></returns>
    public static List<YRouge_Node> TraverseGraphToExtractLowestLeafs(YRouge_Node parentNode)
    {
        Queue<YRouge_Node> NodesToCheck = new Queue<YRouge_Node>();
        List<YRouge_Node> leafNodesToReturn = new List<YRouge_Node>();
        if (parentNode.ChildrenNodeList.Count == 0)
        {
            leafNodesToReturn.Add(parentNode);
        }
        else
        {
            foreach (var childNode in parentNode.ChildrenNodeList)
            {
                NodesToCheck.Enqueue(childNode as YRouge_Node);
            }

            while(NodesToCheck.Count > 0)
            {
                YRouge_Node currentNode = NodesToCheck.Dequeue();
                if (currentNode.ChildrenNodeList.Count == 0)
                {
                    leafNodesToReturn.Add(currentNode);
                }
                else
                {
                    foreach (var childNode in currentNode.ChildrenNodeList)
                    {
                        NodesToCheck.Enqueue(childNode as YRouge_Node);
                    }
                }
            }
            
        }
        return leafNodesToReturn;
    }
/// <summary>
/// 此方法用于生成两个点之间的左下角位置
/// </summary>
/// <param name="bottomLeftPoint"></param>
/// <param name="topRightPoint"></param>
/// <param name="pointModifier"></param>
/// <param name="offset">用于减掉墙壁等</param>
/// <returns></returns>
/// <exception cref="NotImplementedException"></exception>
    public static Vector2Int GenerateBottomLeftCornerBetween
        (Vector2Int boundaryLeftPoint, Vector2Int boundaryRightPoint, float pointModifier, int offset)
    {
        int minX = boundaryLeftPoint.x + offset;
        int minY = boundaryLeftPoint.y + offset;
        int maxX = boundaryRightPoint.x - offset;
        int maxY = boundaryRightPoint.y - offset;
        return new Vector2Int(
            Random.Range(minX,(int)(minX+(maxX-minX)*pointModifier)),
            Random.Range(minY,(int)(minY+(maxY-minY)*pointModifier)));
    }
    public static Vector2Int GenerateTopRightCornerBetween
        (Vector2Int boundaryLeftPoint, Vector2Int boundaryRightPoint, float pointModifier, int offset)
    {
        int minX = boundaryLeftPoint.x + offset;
        int minY = boundaryLeftPoint.y + offset;
        int maxX = boundaryRightPoint.x - offset;
        int maxY = boundaryRightPoint.y - offset;
        return new Vector2Int(
            Random.Range((int)(minX+(maxX-minX)*pointModifier),maxX),
            Random.Range((int)(minY+(maxY-minY)*pointModifier),maxY));
    }
    /// <summary>
    /// 此代码用于计算两个点之间的中间点
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static Vector2Int CalculateMiddlePoint(Vector2Int v1, Vector2Int v2)
    {
        Vector2 sum = v1 + v2;
        Vector2 tempVector = sum / 2;
        return new Vector2Int((int)tempVector.x, (int)tempVector.y);
    }

}
//RelativePosition enum
public enum RelativePosition
{
    Up,
    Down,
    Right,
    Left,
}

//房间类型 战斗房 挑战房 冒险房（枘凿） 游戏机房（对赌）  boss房 道具房 商店房
public enum RoomType
{
    BattleRoom,//战斗房
    ChallengeRoom,//挑战房
    AdventureRoom,//冒险房
    GameRoom,//游戏机房
    BossRoom,//boss房
    ItemRoom,//道具房
    ShopRoom,//商店房
}

public struct StructWithRoomListAndCorridorList
{
    public StructWithRoomListAndCorridorList(List<YRoomNode> roomList, List<YRouge_Node> corridorList)
    {
        this.roomList = roomList;
        this.corridorList = corridorList;
    }
    public List<YRoomNode> roomList;
    public List<YRouge_Node> corridorList;
}

