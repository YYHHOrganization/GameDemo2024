using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YBlendshapeSP : YScreenplayBase
{

    int indexInTimeline;//标注是哪一段bs需要更改
    int characterind;
    public YBlendshapeSP(int characterind,int id,int selectId, HTimelineController timelineController) : base(selectId, timelineController)
    {
        this.id = id;
        this.indexInTimeline = yPlanningTable.Instance.selectSequenceInSelfClass[id];//标注是哪一段bs需要更改 获取当前的indexInSequence  可以是策划表里面定义的
        this.characterind = characterind;
        
        //path = "Prefabs/YAnimation/"+yPlanningTable.Instance.SelectTable[id][selectId];
    }
    public override void OnEnter()
    {
        base.OnEnter();
    }
    
    public override void LoadResourcesAndSetTimeline()
    {
        ////加载资源
        
        //todo:先写死，修改第一个资产，后面改为传入修改timeline的第几个资产

        //timelineController.ChangeBlendshapeWithIndex(indexInTimeline, clip);
        
        //设置
        timelineController.ChangeBlendshapeWithIndex(characterind,indexInTimeline, selectId);
        //timelineController.SetCharacter(go);
    }
    
    

}