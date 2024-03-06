using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YAnimationSP : YScreenplayBase
{
    int indexInSequence;
    public YAnimationSP(int id,int selectId, HTimelineController timelineController) : base(selectId, timelineController)
    {
        this.indexInSequence = yPlanningTable.Instance.selectSequenceInSelfClass[id];//标注是哪一段动画需要更改 获取当前的indexInSequence  可以是策划表里面定义的
        this.id = id;
        //indexInTimeline = yPlanningTable.Instance.animationId2TimelineIndex[id];
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
        timelineController.ChangeAnimationWithIndex(indexInSequence, clip);
        //timelineController.AddAnimationTrackWithIndex(indexInSequence, clip);
        
        //设置
        //timelineController.SetCharacter(go);
    }
    
    

}