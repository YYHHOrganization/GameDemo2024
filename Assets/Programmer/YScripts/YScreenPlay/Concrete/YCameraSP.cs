using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YCameraSP : YScreenplayBase
{
    public YCameraSP(int id,int selectId, HTimelineController timelineController) : base(selectId, timelineController)
    {
        this.id = id;
        path = "Prefabs/YCamera/"+yPlanningTable.Instance.SelectTable[id][selectId];
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
        timelineController.ChangeCinemachineWithIndex(0,go);
        // timelineController.ChangeCharacter(selectId,go);
    }

}
