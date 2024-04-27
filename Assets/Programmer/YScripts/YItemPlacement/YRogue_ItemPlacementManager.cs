using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YRogue_ItemPlacementManager : MonoBehaviour
{
    // private YRogue_ItemPlacementHelper itemPlacementHelper;
    // private YRogue_PropPlacementManager propPlacementManager;

    private static YRogue_ItemPlacementManager _instance;
    public static YRogue_ItemPlacementManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<YRogue_ItemPlacementManager>();
            }
            return _instance;
        }
    }
    //单例模式
    

    private void Start()
    {
        // HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        // HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        // //用一块4*4的地板做测试
        // for (int i = 0; i < 32; i++)
        // {
        //     for (int j = 0; j < 32; j++)
        //     {
        //         floorPositions.Add(new Vector2Int(i, j));
        //         roomPositions.Add(new Vector2Int(i, j));
        //     }
        // }
        //
        // itemPlacementHelper = new YRogue_ItemPlacementHelper(floorPositions, roomPositions);
        // propPlacementManager = FindObjectOfType<YRogue_PropPlacementManager>();
        // propPlacementManager.SetData(itemPlacementHelper);
    }
    //根据给出房间的左上角和右下角，进行物品放置
    // public void SetItemPlacement(Vector2Int leftTop, Vector2Int rightBottom)
    // {
    //     HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
    //     HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
    //     for (int i = leftTop.x; i <= rightBottom.x; i++)
    //     {
    //         for (int j = leftTop.y; j <= rightBottom.y; j++)
    //         {
    //             floorPositions.Add(new Vector2Int(i, j));
    //             roomPositions.Add(new Vector2Int(i, j));
    //         }
    //     }
    //     itemPlacementHelper = new YRogue_ItemPlacementHelper(floorPositions, roomPositions);
    //     propPlacementManager = FindObjectOfType<YRogue_PropPlacementManager>();
    //     propPlacementManager.SetData(itemPlacementHelper);
    // }
    
    //根据给出房间的左下角和右上角，进行物品放置
    public void SetItemPlacement(Vector2Int leftBottom, Vector2Int rightTop)
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        for (int i = leftBottom.x; i <= rightTop.x; i++)
        {
            for (int j = leftBottom.y; j <= rightTop.y; j++)
            {
                floorPositions.Add(new Vector2Int(i, j));
                roomPositions.Add(new Vector2Int(i, j));
            }
        }
        YRogue_ItemPlacementHelper itemPlacementHelper = new YRogue_ItemPlacementHelper(floorPositions, roomPositions);
        YRogue_PropPlacementManager propPlacementManager = FindObjectOfType<YRogue_PropPlacementManager>();
        propPlacementManager.SetData(itemPlacementHelper, transform);//transform后面要改为parent的
    }
}
