using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YDestinationSP : YScreenplayBase
{
    int indexInTimeline;//标注是哪一段bs需要更改
    int characterind;
    public YDestinationSP(int characterind,int id,int selectId, HTimelineController timelineController) : base(selectId, timelineController)
    {
        this.characterind = characterind;
        this.id = id;
        this.indexInTimeline = yPlanningTable.Instance.selectSequenceInSelfClass[id];//标注是哪一段clip需要更改 获取当前的indexInSequence  可以是策划表里面定义的

        //path = "Prefabs/YDestination/"+yPlanningTable.Instance.SelectTable[id][selectId];
    }
    public override void OnEnter()
    {
        base.OnEnter();
    }
    
    public override void LoadResourcesAndSetTimeline()
    {
        ////加载资源
        // GameObject obj = Resources.Load<GameObject>(path);
        //目的地信息应该是一个vector3/一个transform  可以存在策划表里面  最终未来希望玩家通过点击屏幕 选择目的地  所以就存储一个vector3应该ok 
        Debug.Log("目的地信息是"+indexInTimeline +" selectId"+selectId); //34000, 01210
        timelineController.ChangeDestinationWithIndex(characterind,indexInTimeline, selectId);
    }

}
