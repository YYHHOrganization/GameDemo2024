using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YRouge_ChallengeRoom : YRouge_RoomBase
{
    // Start is called before the first frame update
    void Start()
    {
        roomType = RoomType.ChallengeRoom;
        base.Start();
        
        //测试生成一些物品
        GenerateItemPlacement();
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
