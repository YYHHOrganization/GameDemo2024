using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class YRouge_GameRoom : YRouge_RoomBase
{
    
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        roomType = RoomType.GameRoom;
        
        //Debug.Log(roomNode.RoomType);
        string name = "SlotMachineRouge";
        
        var op = Addressables.InstantiateAsync(name);
        GameObject go = op.WaitForCompletion() as GameObject ;
        go.transform.parent = transform;
        go.transform.position = 
            new Vector3((roomNode.BottomLeftAreaCorner.x + roomNode.TopRightAreaCorner.x) / 2, 0,
                (roomNode.BottomLeftAreaCorner.y + roomNode.TopRightAreaCorner.y) / 2)+new Vector3(-400,0,-400);
        
        
        // go.transform.parent = transform;
        // go.transform.localPosition = 
        //     new Vector3((roomNode.BottomLeftAreaCorner.x + roomNode.TopRightAreaCorner.x) / 2, 0,
        //         (roomNode.BottomLeftAreaCorner.y + roomNode.TopRightAreaCorner.y) / 2);
    }

    public override void SetResultOn()
    {
        base.SetResultOn();
        
        
    }

}