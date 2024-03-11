using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class YEffectSP :YScreenplayBase
{
    int indexInTimeline;//标注是哪一段clip需要更改
    //每个selectId对应不同的特效
    public YEffectSP(int id,int selectId, HTimelineController timelineController) : base(selectId, timelineController)
    {
        this.id = id;
        this.selectId = yPlanningTable.Instance.selectSequenceInSelfClass[id];//标注是哪一段clip需要更改 获取当前的indexInSequence  可以是策划表里面定义的
        //path = yPlanningTable.Instance.GetEff(base.selectId);
        //预加载资源
        
        //path = "Prefabs/YEffect/"+selectId;
        
    }
    public override void OnEnter()
    {
        base.OnEnter();
    }
    
    public override void LoadResourcesAndSetTimeline()
    {
        //旧版
        yPlanningTable.Instance.SetAllEffRendererFeatureOff();
        ScriptableRendererFeature unitRendererFeature = yPlanningTable.Instance.GetEffRendererFeature(selectId);
        if (unitRendererFeature == null)
        {
            return;
        }
        timelineController.ChangeSciptableRenderFeatureEffectWithSignal(unitRendererFeature);
        //设置
        //timelineController.ChangeEffect(unitRendererFeature);
    }
}
