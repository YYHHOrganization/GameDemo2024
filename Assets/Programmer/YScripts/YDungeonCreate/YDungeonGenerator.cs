using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class YDungeonGenerator
{

    List<YRoomNode> allSpacesNodes=new List<YRoomNode>();
    
    
    private int dungeonWidth, dungeonLength;
    public YDungeonGenerator(int dungeonWidth, int dungeonLength)
    {
        Debug.Log("Dungeon Generator Created");
        this.dungeonWidth = dungeonWidth;
        this.dungeonLength = dungeonLength;
    }
    // public List<YRouge_Node> CalculateRooms(int maxIterations, int roomWidthMin, int roomLengthMin,float roomBottomCornerModifier,float roomTopCornerModifier,int offset
    // ,int corridorWidth)
    public StructWithRoomListAndCorridorList CalculateRooms(int maxIterations, int roomWidthMin, int roomLengthMin,float roomBottomCornerModifier,float roomTopCornerModifier,int offset
        ,int corridorWidth)
    {
        YRouge_BinarySpacePartitioner bsp = new YRouge_BinarySpacePartitioner(dungeonWidth, dungeonLength);
        allSpacesNodes = bsp.PrepareNodesCollection(maxIterations, roomWidthMin, roomLengthMin);
        List<YRouge_Node> roomSpaces = YRouge_StructureHelper.TraverseGraphToExtractLowestLeafs(bsp.RootNode);
        
        YRouge_RoomGenerator roomGenerator = new YRouge_RoomGenerator(maxIterations, roomWidthMin, roomLengthMin);
        List<YRoomNode> roomLists = roomGenerator.GenerateRoomsInGivenSpaces(roomSpaces,roomBottomCornerModifier,roomTopCornerModifier,offset);
        
        // //创建房间类型
        // YRouge_RoomType roomType = new YRouge_RoomType();
        // roomType.GenerateRoomType(roomLists);
        // //生成道具
        // YRouge_CreateItem createItem = new YRouge_CreateItem();
        // createItem.GenerateRoomScript(roomLists);
        
        YRouge_CorridorGenerator corridorGenerator = new YRouge_CorridorGenerator();
        var corridorList = corridorGenerator.CreateCorridor(allSpacesNodes ,corridorWidth);
        
        return new StructWithRoomListAndCorridorList(roomLists,corridorList);
        // return new List<YRouge_Node>(roomLists).Concat(corridorList).ToList();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
