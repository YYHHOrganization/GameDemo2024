using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class YRouge_CorridorGenerator
{
    public List<YRouge_Node> CreateCorridor(List<YRoomNode> allRoomNodes, int corridorWidth)
    {
        List<YRouge_Node> corridorList = new List<YRouge_Node>();
        //按照树的层级从高到低的顺序遍历
        Queue<YRoomNode> structuresToCheck = new Queue<YRoomNode>
            (allRoomNodes.OrderByDescending(node => node.TreeLayerIndex).ToList());
        while (structuresToCheck.Count > 0)
        {
            YRoomNode currentNode = structuresToCheck.Dequeue();
            if (currentNode.ChildrenNodeList.Count == 0)
            {
                continue;
            }
            YRouge_CorridorNode corridorNode = new YRouge_CorridorNode(currentNode.ChildrenNodeList[0], currentNode.ChildrenNodeList[1], corridorWidth);
            corridorList.Add(corridorNode);
        }
        return  corridorList;
    }
}
