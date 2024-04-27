using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YRogueGraph: MonoBehaviour
{
    private static List<Vector2Int> neighbourse4directions = new List<Vector2Int>()
    {
        new Vector2Int(0, 1), //up
        new Vector2Int(1, 0), //right 
        new Vector2Int(0, -1), //down
        new Vector2Int(-1, 0), //left
    };
    private static List<Vector2Int> neighbourse8directions = new List<Vector2Int>()
    {
        new Vector2Int(0, 1), //up
        new Vector2Int(1, 0), //right 
        new Vector2Int(0, -1), //down
        new Vector2Int(-1, 0), //left
        new Vector2Int(1, 1), //right up
        new Vector2Int(1, -1), //right down
        new Vector2Int(-1, -1), //left down
        new Vector2Int(-1, 1), //left up
    };

    private List<Vector2Int> graph;
    
    public YRogueGraph(IEnumerable<Vector2Int> graph)//IEnumerable<Vector2Int> graph的IEnumerable是一个接口，可以用于遍历集合
    {
        this.graph = new List<Vector2Int>(graph);
    }
    public List<Vector2Int> GetNeighbourse4Directions(Vector2Int startPos)
    {
        return GetNeighbours(startPos, neighbourse4directions);
    }

    public List<Vector2Int> GetNeighbourse8Directions(Vector2Int startPos)
    {
        return GetNeighbours(startPos, neighbourse8directions);
    }

    /// <summary>
    /// 获取邻居，输入一个点，返回这个点的邻居的坐标
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="vector2Ints"></param>
    /// <returns></returns>
    private List<Vector2Int> GetNeighbours(Vector2Int startPos, List<Vector2Int> vector2Ints)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();
        foreach (var direction in vector2Ints)
        {
            Vector2Int neighbour = startPos + direction;
            if (graph.Contains(neighbour))
            {
                neighbours.Add(neighbour);
            }
        }
        return neighbours;
    }
}
