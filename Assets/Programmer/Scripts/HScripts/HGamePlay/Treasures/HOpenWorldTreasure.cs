using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HOpenWorldTreasure : MonoBehaviour
{
    // 每个大世界摆放的宝箱，提供用户交互后的逻辑，依据策划表获得奖励，并显示在UI界面上
    public HOpenWorldTreasureStruct treasure;
    public string testMessage;
    public bool giveMeTreasure = false;

    public void SetTreasure(HOpenWorldTreasureStruct treasureStruct)
    {
        treasure = treasureStruct;
    }

    private void Update()
    {
        if (giveMeTreasure)
        {
            if (treasure!=null)
            {
                treasure.GiveoutTreasures();
                giveMeTreasure = false;
            }
        }
    }
}
