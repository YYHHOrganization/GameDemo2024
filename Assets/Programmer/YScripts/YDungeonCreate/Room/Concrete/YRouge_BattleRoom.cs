using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class YRouge_BattleRoom : YRouge_RoomBase
{
    
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        roomType = RoomType.BattleRoom;
    }
    bool isFirstTimeInRoom = true;
    public override void SetResultOn()
    {
        base.SetResultOn();
        
        //如果是第一次近这个房间就读取配置表，然后生成物品
        //如果不是第一次进这个房间就不生成怪物
        if (isFirstTimeInRoom)
        {
            ReadRoomItem();
            GenerateRoomItem();
            isFirstTimeInRoom = false;
            
            // SetAllDoorsUp();//第一次进入房间门会关
        }
        
        //生成怪物
        
    }

    
}
