using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YRogue_TestItemPlacement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //放置物品
        // test
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        //用一块4*4的地板做测试
        int testSize = 16;//32;
        for (int i = 0; i <testSize; i++)
        {
            for (int j = 0; j < testSize; j++)
            {
                floorPositions.Add(new Vector2Int(i, j));
                roomPositions.Add(new Vector2Int(i, j));
            }
        }
        // floorPositions.Add(new Vector2Int(0, 0));
        // floorPositions.Add(new Vector2Int(1, 0));
        // floorPositions.Add(new Vector2Int(2, 0));
        // floorPositions.Add(new Vector2Int(3, 0));
        // floorPositions.Add(new Vector2Int(4, 0));
        // floorPositions.Add(new Vector2Int(5, 0));
        // floorPositions.Add(new Vector2Int(6, 0));
        // floorPositions.Add(new Vector2Int(7, 0));
        // floorPositions.Add(new Vector2Int(8, 0));
        // floorPositions.Add(new Vector2Int(9, 0));
        // floorPositions.Add(new Vector2Int(10, 0));
        //
        // roomPositions.Add(new Vector2Int(0, 0));
        // roomPositions.Add(new Vector2Int(1, 0));
        // roomPositions.Add(new Vector2Int(2, 0));
        // roomPositions.Add(new Vector2Int(3, 0));
        // roomPositions.Add(new Vector2Int(4, 0));
        // roomPositions.Add(new Vector2Int(5, 0));
        // roomPositions.Add(new Vector2Int(6, 0));
        // roomPositions.Add(new Vector2Int(7, 0));
        // roomPositions.Add(new Vector2Int(8, 0));
        // roomPositions.Add(new Vector2Int(9, 0));
        // roomPositions.Add(new Vector2Int(10, 0));
        
        YRogue_ItemPlacementHelper itemPlacementHelper = new YRogue_ItemPlacementHelper(floorPositions, roomPositions);
        YRogue_PropPlacementManager propPlacementManager = FindObjectOfType<YRogue_PropPlacementManager>();//z这句话的意思是找到场景中的YRogue_PropPlacementManager组件
        propPlacementManager.SetData(itemPlacementHelper, transform);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
