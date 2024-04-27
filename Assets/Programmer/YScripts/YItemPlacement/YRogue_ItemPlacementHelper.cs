using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 辅助物品放置的帮助类，根据房间地板信息和不包括走廊的房间地板信息进行初始化，提供了根据放置类型、最大迭代次数和物品区域大小获取物品放置位置的功能。
/// </summary>
public class YRogue_ItemPlacementHelper
{
    // 存储每种放置类型的所有可用位置
    private Dictionary<PlacementType, HashSet<Vector2Int>> tileByType =
        new Dictionary<PlacementType, HashSet<Vector2Int>>();//根据类型存储地板的位置

    // 房间内不包含走廊的地板格子集合
    private HashSet<Vector2Int> roomFloorNoCorridor;
    
    /// <summary>
    /// 用于辅助物品放置的帮助类，根据房间地板信息和不包括走廊的房间地板信息进行初始化。
    /// </summary>
    /// <param name="roomFloor">包括走廊在内的房间地板位置集合</param>
    /// <param name="roomFloorNoCorridor">不包括走廊的房间地板位置集合</param>
    public YRogue_ItemPlacementHelper(HashSet<Vector2Int> roomFloor, HashSet<Vector2Int> roomFloorNoCorridor)
    {
        YRogueGraph graph = new YRogueGraph(roomFloor);
        this.roomFloorNoCorridor = roomFloorNoCorridor;

        // 遍历每个房间地板位置
        foreach (var position in roomFloorNoCorridor)
        {
            int neighboursCount8Dir = graph.GetNeighbourse8Directions(position).Count;
            //如果周围有墙，那么就是NearWall，否则是OpenSpace
            PlacementType type = neighboursCount8Dir < 8 ? PlacementType.NearWall : PlacementType.OpenSpace;
            if (!tileByType.ContainsKey(type))
            {
                tileByType[type] = new HashSet<Vector2Int>();
            }
            // 对于靠墙的位置，如果有4个方向的邻居，则跳过该位置
            if(type == PlacementType.NearWall && graph.GetNeighbourse4Directions(position).Count == 4)continue;

            tileByType[type].Add(position);
        }
    }
    
    /// <summary>
    /// 根据放置类型、最大迭代次数和物品区域大小获取物品放置位置。
    /// </summary>
    /// <param name="placementType">放置类型</param>
    /// <param name="iterationsMax">最大迭代次数</param>
    /// <param name="itemAreaSize">物品区域大小</param>
    /// <returns>物品放置位置的二维向量，如果找不到合适位置则返回null</returns>
    public Vector2? GetItemPlacementPosition(PlacementType placementType, int iterationsMax, Vector2Int itemAreaSize, bool addOffset)
    {
        // 物品区域的大小 面积（以格子为单位）
        int itemArea = itemAreaSize.x * itemAreaSize.y;
        // 如果指定放置类型的可用位置数量小于物品区域的大小，则无法放置，返回null
        if (tileByType[placementType].Count < itemArea)
        {
            return null;
        }

        int iteration = 0;
        while (iteration < iterationsMax)
        {
            iteration++;
                
            // 随机选择一个位置
            int index = UnityEngine.Random.Range(0, tileByType[placementType].Count);
            // if(tileByType[placementType] == null) return null; 
            var position = tileByType[placementType].ElementAtOrDefault(index);//ElementAtOrDefault(index)返回指定索引处的元素；如果索引超出范围，则返回默认值
            if (position == null)
            {
                continue; // 集合中没有指定索引的位置
            }
            // Vector2Int position = tileByType[placementType].ElementAt(index);

            // 如果物品区域大小大于1，则尝试放置较大的物品
            if (itemArea > 1)
            {
                var (result, placementPositions) = PlaceBigItem(position, itemAreaSize, addOffset);
                if (result == false)
                {
                    continue; // 放置失败，进行下一次迭代
                }
                // 从放置类型和邻近墙壁的位置集合中排除已放置的位置
                tileByType[placementType].ExceptWith(placementPositions);
                tileByType[PlacementType.NearWall].ExceptWith(placementPositions);
            }
            else
            {
                // 移除单个位置
                tileByType[placementType].Remove(position);
            }
            return position; // 返回成功放置的位置
        }
        return null; // 达到最大迭代次数仍未找到合适位置，返回null
    }
    /// <summary>
    /// 放置较大物品，返回放置是否成功以及放置的位置列表。
    /// </summary>
    /// <param name="originPosition">起始位置</param>
    /// <param name="size">物品尺寸</param>
    /// <param name="addOffset">是否添加偏移量</param>
    /// <returns>放置是否成功以及放置的位置列表</returns>
    private (bool, List<Vector2Int>) PlaceBigItem(Vector2Int originPosition, Vector2Int size, bool addOffset)
    {
        // 初始化放置的位置列表，并加入起始位置
        List<Vector2Int> positions = new List<Vector2Int>() { originPosition };

        // 计算边界值
        int maxX = addOffset ? size.x + 1 : size.x;
        int maxY = addOffset ? size.y + 1 : size.y;
        int minX = addOffset ? -1 : 0;
        int minY = addOffset ? -1 : 0;

        // 遍历每个位置
        for (int row = minX; row <= maxX; row++)
        {
            for (int col = minY; col <= maxY; col++)
            {
                // 跳过起始位置
                if (col == 0 && row == 0)
                {
                    continue;
                }

                // 计算新位置
                Vector2Int newPosToCheck = new Vector2Int(originPosition.x + row, originPosition.y + col);

                // 检查新位置是否可用
                if (roomFloorNoCorridor.Contains(newPosToCheck) == false)
                {
                    return (false, positions); // 放置失败，返回失败状态和已放置的位置列表
                }
                positions.Add(newPosToCheck); // 将新位置加入已放置的位置列表
            }
        }
        return (true, positions); // 放置成功，返回成功状态和已放置的位置列表
    }
}

public enum PlacementType
{
    OpenSpace,
    NearWall
}