using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YAnimationSP : YScreenplayBase
{
    public YAnimationSP(int selectId, HTimelineController timelineController) : base(selectId, timelineController)
    {
        id = 1;
        path = "Prefabs/YAnimation/"+yPlanningTable.Instance.SelectTable[id][selectId];
    }
    public override void OnEnter()
    {
        base.OnEnter();
    }
    
    public override void LoadResourcesAndSetTimeline()
    {
        ////加载资源
        //加载动画clip资源
        
        //todo:先写死，修改第一个动画资产，后面改为传入修改timeline的第几个动画资产
        AnimationClip clip = Resources.Load<AnimationClip>(path);
        timelineController.ChangeAnimationWithIndex(1, clip);
        
        //设置
        //timelineController.SetCharacter(go);
    }
}
