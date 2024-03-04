using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YTreasureSP : YScreenplayBase
{
    public YTreasureSP(int selectId, HTimelineController timelineController) : base(selectId, timelineController)
    {
        id = 7;
        path = "Prefabs/YTreasure/treasure"+yPlanningTable.Instance.SelectTable[id][selectId];
    }
    public override void OnEnter()
    {
        base.OnEnter();
    }
    
    public override void LoadResourcesAndSetTimeline()
    {
        ////加载资源
        //GameObject obj = Resources.Load<GameObject>(path);
        GameObject go = GameObject.Instantiate(Resources.Load<GameObject>(path));
        timelineController.ChangeCertainObjectWithName("Movie",go);
        
        timelineController.ChangeTresureWithIndex(0,path);
        // timelineController.ChangeCharacter(selectId,go);
    }

}
