using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class YDungeonCreator : MonoBehaviour
{
    public int dungeonWidth, dungeonLength;
    public int roomWidthMin, roomLengthMin;
    public int maxIterations;//迭代次数
    public int coridorWidth;//走廊宽度
    public Material floorMaterial;
    public Material RedFloorMaterial;
    public Material HuazhuanFloorMaterial_BlanckWhite;
    public Material HuazhuanFloorMaterial_Hua;
    public Material HuazhuanFloorMaterial_;
    public Material BornRoomFloorMaterial;
    [Range(0.0f,0.3f)]
    public float roomBottomCornerModifier;
    [Range(0.7f,1.0f)]
    public float roomTopCornerModifier;
    [Range(0,2)]
    public int offset;
    public GameObject wallVertical, wallHorizontal;
    public GameObject doorVertical, doorHorizontal;
    
    public GameObject RedWallVertical, RedWallHorizontal;
    public GameObject RedOldWallVertical, RedOldWallHorizontal;
    
    private List<Vector3Int> possibleDoorVerticalPositions;//可能的门的位置
    private List<Vector3Int> possibleDoorHorizontalPositions;
    //wall
    private List<Vector3Int> possibleWallVerticalPositions;
    private List<Vector3Int> possibleWallHorizontalPositions;
    
    //更新为第一列是房间的编号,
    private List<List<Vector3Int>> roomWallHorizontalPositions;
    private List<List<Vector3Int>> roomWallVerticalPositions;
    private List<List<Vector3Int>> roomDoorHorizontalPositions;
    private List<List<Vector3Int>> roomDoorVerticalPositions;
    
    List<Vector3Int> corridorWallHorizontalPositions;
    List<Vector3Int> corridorWallVerticalPositions;
    
    public Vector3Int originPosition;
    
    LayerMask floorLayerMask;
    // Start is called before the first frame update
    void Start()
    {
        // CreateDungeon();
        //监听是否开始新的一关
        YTriggerEvents.OnEnterNewLevel += EnterNewLevel;
        floorLayerMask = LayerMask.NameToLayer("RogueFloor");//提前存储节省性能
    }

    void EnterNewLevel(object sender, YTriggerEventArgs e)
    {
        BakeNavMesh();
    }
    
    List<YRoomNode> roomList ;
    List<YRouge_Node> corridorList ;
    
    List<YRouge_RoomBase> roomBaseList;
    public void CreateDungeon(int style=0 ,bool testGenerateIcon=false)
    {
        DestroyAllChildren();
        
        YDungeonGenerator generator = new YDungeonGenerator(dungeonWidth, dungeonLength);
        //以下的CalculateRooms同样生成了走廊的结构
        StructWithRoomListAndCorridorList ListOfRooms = generator.CalculateRooms(
            maxIterations,
            roomWidthMin, 
            roomLengthMin,
            roomBottomCornerModifier, 
            roomTopCornerModifier, 
            offset,
            coridorWidth);
        roomList = ListOfRooms.roomList;
        corridorList = ListOfRooms.corridorList;
        
        //暂存一下space 用于mask
        roomSpacesKeepList = generator.GetRoomSpacesKeep();
        
        //创建房间类型
        YRouge_RoomType roomType = new YRouge_RoomType();
        roomType.GenerateRoomType(roomList);
        //生成Room的脚本
        YRouge_CreateItem createItem = new YRouge_CreateItem();
        roomBaseList = createItem.GenerateRoomScript(roomList,this.transform,originPosition,roomSpacesKeepList);
        
        GameObject wallParent = new GameObject("WallParent");
        wallParent.transform.parent = transform;
        possibleDoorHorizontalPositions = new List<Vector3Int>();
        possibleDoorVerticalPositions = new List<Vector3Int>();
        possibleWallHorizontalPositions = new List<Vector3Int>();
        possibleWallVerticalPositions = new List<Vector3Int>();
        
        //更新为第一列是房间的编号，大小为房间的数量
        roomWallHorizontalPositions = new List<List<Vector3Int>>();
        roomWallVerticalPositions = new List<List<Vector3Int>>();
        roomDoorHorizontalPositions = new List<List<Vector3Int>>();
        roomDoorVerticalPositions = new List<List<Vector3Int>>();
        
        corridorWallHorizontalPositions = new List<Vector3Int>();
        corridorWallVerticalPositions = new List<Vector3Int>();
        
        //创建地板
        for(int i = 0; i < roomList.Count; i++)
        {
            createRoomMeshes(i,roomList[i],roomList[i].BottomLeftAreaCorner, roomList[i].TopRightAreaCorner);
        }
        // foreach (var room in roomList)
        // {
        //     createRoomMeshes(room,room.BottomLeftAreaCorner, room.TopRightAreaCorner);
        // }
        foreach (var corridor in corridorList)
        {
            createMeshes(corridor.BottomLeftAreaCorner, corridor.TopRightAreaCorner);
        }
        
        //不应该在这里创建墙壁和门，应该在房间的脚本中创建？或者应该把这个每个房间自己的door和wall的list传给这个类，然后这个类再创建
        // CreateWalls(wallParent);
        if(style==1)return;
        
        CreateRoomWalls();
        GameObject doorParent = new GameObject("DoorParent");
        doorParent.transform.parent = transform;
        // CreateDoors(doorParent);
        CreateRoomDoor();
        GameObject CorridorParent = new GameObject("CorridorParent");
        CorridorParent.transform.parent = transform;
        CrrateCorridorWall(CorridorParent.transform);
        
        if(testGenerateIcon)
        {
            TestGenerateAllIcon();
        }
        //移动到（-400，0，-400）
        //transform.position = new Vector3(-400, 0, -400);
        // BakeNavMesh();
    }
    //测试用的代码 调用每个房间的脚本的GenerateLittleMapMask(); 显示出图标
    public void TestGenerateAllIcon()
    {
        //foreach (var roomBase in roomBaseList)
        for(int i = 0; i < roomBaseList.Count; i++)
        {
            YRouge_RoomBase roomBase = roomBaseList[i];
            RoomType roomType = roomBase.roomNode.RoomType;
            roomBase.GenerateIcon(roomType);
            //将命名改为第几个
            roomBase.gameObject.name = "Room_"+i;
            
            //Debug输出每个房间的邻居
            Debug.Log("Room_"+i+"的邻居有:");
            foreach (var neighbor in roomBase.roomNode.Neighbors)
            {
                YRoomNode neighbor1 = neighbor as YRoomNode;
                Debug.Log(neighbor1.RoomType);
            }
            
        }
        
    }
    
    
    public List<YRouge_RoomBase> GetRoomBaseList()
    {
        return roomBaseList;
    }
    //NavMesh()
    public NavMeshSurface surface;
    public void BakeNavMesh()
    {
        if (surface)
        {
            surface.BuildNavMesh();
        }
    }
    

    private void CrrateCorridorWall(Transform CorridorParent)
    {
        for (int i = 0; i < corridorWallHorizontalPositions.Count; i++)
        {
            CreateWall(corridorWallHorizontalPositions[i], CorridorParent, wallHorizontal);
        }
        for (int i = 0; i < corridorWallVerticalPositions.Count; i++)
        {
            CreateWall(corridorWallVerticalPositions[i], CorridorParent, wallVertical);
        }
    }

    /// <summary>
    /// 遍历所有房间，创建房间的墙壁
    /// </summary>
    void CreateRoomWalls()
    {
        for (int i = 0; i < roomWallHorizontalPositions.Count; i++)
        {
            for(int j = 0; j < roomWallHorizontalPositions[i].Count; j++)
            {
                GameObject wallParent = roomList[i].roomScript.gameObject;
                //根据不同的房间，创建不同的墙壁
                GameObject mWallPrefab = GetPrefabFromRoomType(roomList[i].mRoomType, true);
               
                CreateWall(roomWallHorizontalPositions[i][j], wallParent, mWallPrefab,i);
            }
        }
        for (int i = 0; i < roomWallVerticalPositions.Count; i++)
        {
            for(int j = 0; j < roomWallVerticalPositions[i].Count; j++)
            {
                GameObject wallParent = roomList[i].roomScript.gameObject;
                //根据不同的房间，创建不同的墙壁
                GameObject mWallPrefab = GetPrefabFromRoomType(roomList[i].mRoomType, false);

                CreateWall(roomWallVerticalPositions[i][j], wallParent, mWallPrefab, i);
            }
        }
    }

    private GameObject GetPrefabFromRoomType(RoomType mRoomType, bool isHorizontal)
    {
        GameObject wallPrefab = null;
        if(mRoomType == RoomType.BattleRoom)
        {
            if (isHorizontal) return RedOldWallHorizontal;
            else return RedOldWallVertical;
            
        }
        if(mRoomType == RoomType.BossRoom)
        {
            if (isHorizontal) return RedWallHorizontal;
            else return RedWallVertical;
        }
        else 
        {
            if (isHorizontal) return wallHorizontal;
            else return wallVertical;
        }
        return wallPrefab;
    }
    
    private Material GetFloorMatFromRoomType(RoomType roomMRoomType)
    {
        if(roomMRoomType == RoomType.BattleRoom)
        {
            return RedFloorMaterial;
        }
        else if(roomMRoomType == RoomType.BossRoom)
        {
            return HuazhuanFloorMaterial_Hua;
        }
        else if(roomMRoomType == RoomType.ItemRoom)
        {
            return HuazhuanFloorMaterial_BlanckWhite;
        }
        else if(roomMRoomType == RoomType.ShopRoom)
        {
            return HuazhuanFloorMaterial_;
        }
        else if(roomMRoomType == RoomType.BornRoom)
        {
            return BornRoomFloorMaterial;
        }
        
        return floorMaterial;
    }
    void CreateRoomDoor()
    {
        for (int i = 0; i < roomDoorHorizontalPositions.Count; i++)
        {
            for(int j = 0; j < roomDoorHorizontalPositions[i].Count; j++)
            {
                GameObject doorParent = roomList[i].roomScript.gameObject;
                CreateHorizontalDoor(roomDoorHorizontalPositions[i][j], doorParent, doorHorizontal, i);
            }
        }
        for (int i = 0; i < roomDoorVerticalPositions.Count; i++)
        {
            for(int j = 0; j < roomDoorVerticalPositions[i].Count; j++)
            {
                GameObject doorParent = roomList[i].roomScript.gameObject;
                CreateVerticalDoor(roomDoorVerticalPositions[i][j], doorParent, doorVertical, i);
            }
        }
    }

    private void CreateHorizontalDoor(Vector3Int doorPosition, GameObject doorParent, GameObject doorPrefab, int id)
    {
        // GameObject door = Instantiate(doorPrefab, doorPosition, Quaternion.identity,doorParent.transform);
        GameObject door = Instantiate(doorPrefab, doorPosition+originPosition, Quaternion.identity,doorParent.transform);
        //将wall存入他相应的房间的wallList中
        YRouge_RoomBase roomBase = roomList[id].roomScript;
        if (roomBase != null)
        {
            roomBase.horizontaldoors.Add(door);
        }
    }
    private void CreateVerticalDoor(Vector3Int doorPosition, GameObject doorParent, GameObject doorPrefab, int id)
    {
        // GameObject door = Instantiate(doorPrefab, doorPosition, Quaternion.identity,doorParent.transform);
        GameObject door = Instantiate(doorPrefab, doorPosition+originPosition, Quaternion.identity,doorParent.transform);
        //将wall存入他相应的房间的wallList中
        YRouge_RoomBase roomBase = roomList[id].roomScript;
        if (roomBase != null)
        {
            roomBase.vertiacaldoors.Add(door);
        }
        
    }
    
    private void CreateWall(Vector3Int wallPosition, Transform wallParent, GameObject wallPrefab)
    {
        GameObject wall = Instantiate(wallPrefab, wallPosition+originPosition, Quaternion.identity,wallParent);
        //将wall存入他相应的房间的wallList中
    }
    
    /// <summary>
    /// 创建墙壁，每个房间的墙壁都是独立的，加入到自己/YRouge_RoomBase的wallList中
    /// </summary>
    /// <param name="wallPosition"></param>
    /// <param name="wallParent"></param>
    /// <param name="wallPrefab"></param>
    /// <param name="id"></param>
    private void CreateWall(Vector3Int wallPosition, GameObject wallParent, GameObject wallPrefab,int id)
    {
        GameObject wall = Instantiate(wallPrefab, wallPosition+originPosition, Quaternion.identity,wallParent.transform);
        //将wall存入他相应的房间的wallList中
        YRouge_RoomBase roomBase = roomList[id].roomScript;
        if (roomBase != null)
        {   
            roomBase.walls.Add(wall);
        }
    }
   
    private void createRoomMeshes(int id,YRoomNode room, Vector2 bottomLeftCorner, Vector2 topRightCorner)
    {
        roomWallHorizontalPositions.Add(new List<Vector3Int>());
        roomWallVerticalPositions.Add(new List<Vector3Int>());
        roomDoorHorizontalPositions.Add(new List<Vector3Int>());
        roomDoorVerticalPositions.Add(new List<Vector3Int>());
            
        Vector3 bottomLeft = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRight = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
        Vector3 topRight = new Vector3(topRightCorner.x, 0, topRightCorner.y);
        Vector3 topLeft = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
        
        //以下表示一个矩形的四个顶点
        Vector3[] vertices = new Vector3[] {topLeft, topRight, bottomLeft, bottomRight};
        Vector2[] uvs = new Vector2[vertices.Length];//纹理坐标
        //以下表示一个矩形的四个顶点的纹理坐标
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }
        // 逆时针
        int[] triangles = new int[] { 0,1,2,2,1,3};
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        
        //it means that the mesh will be rendered with the material floorMaterial
        GameObject floor = new GameObject("Mesh"+bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer));
        
        floor.transform.position = originPosition;
        floor.transform.localScale = new Vector3(1, 1, 1);
        
        // floor.transform.position = new Vector3(0, 0, 0);
        //获取中心位置
        GameObject roomparent = room.roomScript.gameObject;
        floor.transform.parent = roomparent.transform;
        Vector3 meshCenterPos = new Vector3((bottomLeftCorner.x + topRightCorner.x) / 2, 0, (bottomLeftCorner.y + topRightCorner.y) / 2) + originPosition;
        floor.transform.RotateAround( meshCenterPos, Vector3.right, 180);
        
        //根据不同房间类型，替换不同房间地板材质
        Material floorMat = GetFloorMatFromRoomType(room.mRoomType);
        
        //绕着自身中心点旋转180度
        floor.GetComponent<MeshFilter>().mesh = mesh;
        floor.GetComponent<MeshRenderer>().material = floorMat;
        floor.transform.parent = transform;

        YRouge_RoomBase roomBase = room.roomScript;
        if (roomBase != null)
        {
            roomBase.Floor = floor;
        }
        
        for(int row =(int)bottomLeft.x; row < topRight.x; row++)
        {
            var point = new Vector3(row, 0, bottomLeft.z);
            // AddWallPositionToList(point, possibleWallHorizontalPositions, possibleDoorHorizontalPositions);
            // AddRoomWallPositionToList(point,  possibleWallHorizontalPositions);
            AddRoomWallPositionToList(point, roomWallHorizontalPositions[id]);
        }
        for(int row =(int)topLeft.x; row < topRight.x; row++)
        {
            var point = new Vector3(row, 0, topRight.z);
            // AddWallPositionToList(point, possibleWallHorizontalPositions, possibleDoorHorizontalPositions);
            //AddRoomWallPositionToList(point,  possibleWallHorizontalPositions);
            AddRoomWallPositionToList(point, roomWallHorizontalPositions[id]);
        }
        for(int col =(int)bottomLeft.z; col < topLeft.z; col++)
        {
            var point = new Vector3(bottomLeft.x, 0, col);
            // AddWallPositionToList(point, possibleWallVerticalPositions, possibleDoorVerticalPositions);
            // AddRoomWallPositionToList(point,  possibleWallVerticalPositions);
            AddRoomWallPositionToList(point, roomWallVerticalPositions[id]);
        }
        for(int col =(int)bottomRight.z; col < topRight.z; col++)
        {
            var point = new Vector3(bottomRight.x, 0, col);
            // AddWallPositionToList(point, possibleWallVerticalPositions, possibleDoorVerticalPositions);
            // AddRoomWallPositionToList(point,  possibleWallVerticalPositions);
            AddRoomWallPositionToList(point, roomWallVerticalPositions[id]);
        }
        SetFloorLayer(floor);
    }

   

    //每个房间单独存储自己的wallList（和doorList
    private void AddRoomWallPositionToList(Vector3 point, List<Vector3Int> wallList)
    {
        Vector3Int pointInt = Vector3Int.CeilToInt(point);//向上取整
        wallList.Add(pointInt);
    }
    
    private void createMeshes(Vector2 bottomLeftCorner,Vector2 topRightCorner)
    {
        Vector3 bottomLeft = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRight = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
        Vector3 topRight = new Vector3(topRightCorner.x, 0, topRightCorner.y);
        Vector3 topLeft = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
        
        //以下表示一个矩形的四个顶点
        Vector3[] vertices = new Vector3[] {topLeft, topRight, bottomLeft, bottomRight};
        Vector2[] uvs = new Vector2[vertices.Length];//纹理坐标
        //以下表示一个矩形的四个顶点的纹理坐标
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }
        int[] triangles = new int[] {0, 1, 2, 2, 1, 3};//三角形顶点索引,这里表示两个三角形,一个是012,另一个是213,这样就构成了一个矩形
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        
        //it means that the mesh will be rendered with the material floorMaterial
        GameObject floor = new GameObject("Mesh"+bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer));
        
        // floor.transform.position = new Vector3(0, 0, 0);
        floor.transform.position = originPosition;
        floor.transform.localScale = new Vector3(1, 1, 1);
        floor.GetComponent<MeshFilter>().mesh = mesh;
        floor.GetComponent<MeshRenderer>().material = floorMaterial;
        floor.transform.parent = transform;
        
        //每个房间有自己的wallList和doorList
        
        for(int row =(int)bottomLeft.x; row < topRight.x; row++)
        {
            var point = new Vector3(row, 0, bottomLeft.z);
            // AddWallPositionToList(point, possibleWallHorizontalPositions, possibleDoorHorizontalPositions);
            AddCorriWallPositionToList(point, roomWallHorizontalPositions, roomDoorHorizontalPositions, corridorWallHorizontalPositions);
        }
        for(int row =(int)topLeft.x; row < topRight.x; row++)
        {
            var point = new Vector3(row, 0, topRight.z);
            // AddWallPositionToList(point, possibleWallHorizontalPositions, possibleDoorHorizontalPositions);
            AddCorriWallPositionToList(point, roomWallHorizontalPositions, roomDoorHorizontalPositions, corridorWallHorizontalPositions);
        }
        for(int col =(int)bottomLeft.z; col < topLeft.z; col++)
        {
            var point = new Vector3(bottomLeft.x, 0, col);
            // AddWallPositionToList(point, possibleWallVerticalPositions, possibleDoorVerticalPositions);
            AddCorriWallPositionToList(point, roomWallVerticalPositions, roomDoorVerticalPositions, corridorWallVerticalPositions);
        }
        for(int col =(int)bottomRight.z; col < topRight.z; col++)
        {
            var point = new Vector3(bottomRight.x, 0, col);
            // AddWallPositionToList(point, possibleWallVerticalPositions, possibleDoorVerticalPositions);
            AddCorriWallPositionToList(point, roomWallVerticalPositions, roomDoorVerticalPositions, corridorWallVerticalPositions);
        }
        SetFloorLayer(floor);
    }
    
    
    void SetFloorLayer(GameObject floor)
    {
        floor.layer = floorLayerMask;
    }
    //房间和走廊原本都被wall包围，但是当二者的wall重叠后，应该是door的位置，wall被door替换，wall的位置就会被door占据
    private void AddWallPositionToList(Vector3 point, List<Vector3Int> wallList, List<Vector3Int> doorList)
    {
        Vector3Int pointInt = Vector3Int.CeilToInt(point);//向上取整
        if (wallList.Contains(pointInt) )
        {
            wallList.Remove(pointInt);
            doorList.Add(pointInt);
        }
        else
        {
            wallList.Add(pointInt);
        }
        
    }
    private void AddCorriWallPositionToList(
        Vector3 point,List<List<Vector3Int>> roomWalls, 
        List<List<Vector3Int>> roomDoors,
            List<Vector3Int> corriWallList)
    {
        Vector3Int pointInt = Vector3Int.CeilToInt(point);//向上取整
        //循环遍历所有roomWalls的walllist，如果有重叠的，就把这个点加入到doorlist中
        for (int i = 0; i < roomWalls.Count; i++)
        {
            //如果这个点在房间的wallList中,说明这是个门
            if (roomWalls[i].Contains(pointInt))
            {
                roomWalls[i].Remove(pointInt);
                roomDoors[i].Add(pointInt);
                break;//找到一个就行 说明这个点是门
            }
            else if(i==roomWalls.Count-1)//直到最后一个房间的wallList都没有这个点，说明这个点是走廊的墙
            {
                corriWallList.Add(pointInt);
            }
            
        }
        
        
    }
    private void DestroyAllChildren()
    {
        while(transform.childCount > 0)
        {
            foreach(Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }

    List<roomSpaceKeep> roomSpacesKeepList;
    public List<roomSpaceKeep> GetRoomSpacesKeep()
    {
        return roomSpacesKeepList;
    }
}
