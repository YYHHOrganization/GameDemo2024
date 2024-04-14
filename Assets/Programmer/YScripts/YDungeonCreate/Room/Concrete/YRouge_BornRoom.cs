using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YRouge_BornRoom : YRouge_RoomBase
{
    // Start is called before the first frame update
    void Start()
    {
        roomType = RoomType.BornRoom;
        base.Start();
        
        roomLittleMapMask.SetActive(false);
        //更新一下出生位置
        GameObject goBornPlace = new GameObject();
        goBornPlace.name = "RogueBornPlace";
        goBornPlace.transform.parent = transform;
        goBornPlace.transform.localPosition = new Vector3(0, 0, 0);
        //yPlanningTable.Instance.UpdateCharacterGeneratePlace(3,"RogueBornPlace");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
