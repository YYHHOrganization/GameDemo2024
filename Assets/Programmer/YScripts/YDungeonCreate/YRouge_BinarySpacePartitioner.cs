using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YRouge_BinarySpacePartitioner 
{
    YRoomNode rootNode;

    public YRoomNode RootNode{get=>rootNode;}
    Vector2Int dungeonBottomLeftAreaCorner = new Vector2Int(0, 0);//暂时设定为0,0
    public YRouge_BinarySpacePartitioner(int width, int length)
    {
        Debug.Log("Binary Space Partitioner Created");
        this.rootNode = new YRoomNode(dungeonBottomLeftAreaCorner,
            new Vector2Int(width, length), null, 0);
        
    }
    public List<YRoomNode> PrepareNodesCollection(int maxIterations, int roomWidthMin, int roomLengthMin)
    {
        Queue<YRoomNode> graph = new Queue<YRoomNode>();
        graph.Enqueue(this.rootNode);
        List<YRoomNode> listToReturn = new List<YRoomNode>();
        listToReturn.Add(this.rootNode);
        int iterations = 0;
        while (graph.Count > 0 && iterations < maxIterations)
        {
            iterations++;
            YRoomNode currentNode = graph.Dequeue();
            if (currentNode.RoomWidth >= roomWidthMin * 2 || currentNode.RoomLength >= roomLengthMin * 2)
            {
                SplitTheSpace(currentNode, listToReturn, graph, roomWidthMin, roomLengthMin);
            }
            
        }
        return listToReturn;
    }

    private void SplitTheSpace(YRoomNode currentNode, List<YRoomNode> listToReturn, Queue<YRoomNode> graph, int roomWidthMin, int roomLengthMin)
    {
        YRouge_Line line = GetLineDivingTheSpace(
            currentNode.BottomLeftAreaCorner, 
            currentNode.TopRightAreaCorner,
            roomWidthMin, 
            roomLengthMin);
        YRoomNode node1, node2;
        if(line.Orientation==Orientation.Horizontal)
        {
            node1 = new YRoomNode(
                currentNode.BottomLeftAreaCorner, 
                new Vector2Int(currentNode.TopRightAreaCorner.x, line.Coordinate.y), 
                currentNode, 
                currentNode.TreeLayerIndex+1);
            node2 = new YRoomNode(
                new Vector2Int(currentNode.BottomLeftAreaCorner.x, line.Coordinate.y), 
                currentNode.TopRightAreaCorner, 
                currentNode, 
                currentNode.TreeLayerIndex+1);
        }
        else
        {
            node1 = new YRoomNode(currentNode.BottomLeftAreaCorner, 
                new Vector2Int(line.Coordinate.x, currentNode.TopRightAreaCorner.y), 
                currentNode, 
                currentNode.TreeLayerIndex+1);
            node2 = new YRoomNode(new Vector2Int(line.Coordinate.x, currentNode.BottomLeftAreaCorner.y), 
                currentNode.TopRightAreaCorner, 
                currentNode, 
                currentNode.TreeLayerIndex+1);
        }
        
        AddNewNodeToGraph(node1, graph, listToReturn);
        AddNewNodeToGraph(node2, graph, listToReturn);
        
    }

    private void AddNewNodeToGraph(YRoomNode node1, Queue<YRoomNode> graph, List<YRoomNode> listToReturn)
    {
        graph.Enqueue(node1);
        listToReturn.Add(node1);
    }

    private YRouge_Line GetLineDivingTheSpace(Vector2 currentNodeBottomLeftAreaCorner, Vector2 currentNodeTopRightAreaCorner, int roomWidthMin, int roomLengthMin)
    {
        Orientation orientation;
        bool lengthStatus = currentNodeTopRightAreaCorner.y - currentNodeBottomLeftAreaCorner.y >= roomLengthMin * 2;
        bool widthStatus = currentNodeTopRightAreaCorner.x - currentNodeBottomLeftAreaCorner.x >= roomWidthMin * 2;
        if (lengthStatus && widthStatus)
        {
            orientation = Random.Range(0, 2) == 0 ? Orientation.Horizontal : Orientation.Vertical;
        }
        else if (lengthStatus)
        {
            orientation = Orientation.Horizontal;
        }
        else if (widthStatus)
        {
            orientation = Orientation.Vertical;
        }
        else
        {
            return null;
        }
        return new YRouge_Line(orientation, 
            GetCoordinateFororientation(
                currentNodeBottomLeftAreaCorner, 
                currentNodeTopRightAreaCorner, 
                orientation, 
                roomWidthMin, 
                roomLengthMin));
    }

    //以下方法是为了获取分割线的坐标 （Fororientation英语翻译是：为了方向 即 for orientation
    private Vector2Int GetCoordinateFororientation(Vector2 currentNodeBottomLeftAreaCorner, Vector2 currentNodeTopRightAreaCorner, Orientation orientation, int roomWidthMin, int roomLengthMin)
    {
        Vector2Int coordinate = Vector2Int.zero;
        if (orientation == Orientation.Horizontal)
        {
            coordinate= new Vector2Int(0,
                Random.Range(
                    (int)currentNodeBottomLeftAreaCorner.y+roomLengthMin, 
                    (int)currentNodeTopRightAreaCorner.y-roomLengthMin));
        }
        else
        {
            coordinate = new Vector2Int(
                Random.Range(
                    (int)currentNodeBottomLeftAreaCorner.x+roomWidthMin, 
                    (int)currentNodeTopRightAreaCorner.x-roomWidthMin),
                0);
        }
        return coordinate;
    }
    
}