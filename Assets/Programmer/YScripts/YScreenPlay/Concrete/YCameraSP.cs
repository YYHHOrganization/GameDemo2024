using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YCameraSP : YScreenplayBase
{
    int indexInSequence;
    string name;
    public YCameraSP(int id,int selectId, HTimelineController timelineController) : base(selectId, timelineController)
    {
        this.indexInSequence = yPlanningTable.Instance.selectSequenceInSelfClass[id];//标注是哪一段动画需要更改 获取当前的indexInSequence  可以是策划表里面定义的
        this.id = id;
        path = "Prefabs/YCamera/"+yPlanningTable.Instance.SelectTable[id][selectId];
        name = yPlanningTable.Instance.SelectTable[id][selectId];
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
        // timelineController.ChangeCinemachineWithIndex(indexInSequence,go);
        timelineController.ChangeCinemachine(indexInSequence,go,name);

    }

}
