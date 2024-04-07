using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YDungeonGenerator
{

    List<YRoomNode> allRoomNodes=new List<YRoomNode>();
    
    
    private int dungeonWidth, dungeonLength;
    public YDungeonGenerator(int dungeonWidth, int dungeonLength)
    {
        Debug.Log("Dungeon Generator Created");
        this.dungeonWidth = dungeonWidth;
        this.dungeonLength = dungeonLength;
    }
    public List<YRoomNode> CalculateRooms(int maxIterations, int roomWidthMin, int roomLengthMin,float roomBottomCornerModifier,float roomTopCornerModifier,int offset
    ,int corridorWidth)
    {
        YRouge_BinarySpacePartitioner bsp = new YRouge_BinarySpacePartitioner(dungeonWidth, dungeonLength);
        allRoomNodes = bsp.PrepareNodesCollection(maxIterations, roomWidthMin, roomLengthMin);
        List<YRouge_Node> roomSpaces = YRouge_StructureHelper.TraverseGraphToExtractLowestLeafs(bsp.RootNode);
        YRouge_RoomGenerator roomGenerator = new YRouge_RoomGenerator(maxIterations, roomWidthMin, roomLengthMin);
        List<YRoomNode> roomLists = roomGenerator.GenerateRoomsInGivenSpaces(roomSpaces,roomBottomCornerModifier,roomTopCornerModifier,offset);
        
        YRouge_CorridorGenerator corridorGenerator = new YRouge_CorridorGenerator();
        var corridorList = corridorGenerator.CreateCorridor(allRoomNodes ,corridorWidth);
        return new List<YRoomNode>(roomLists);
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
