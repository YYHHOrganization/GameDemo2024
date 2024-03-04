using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YAudioSP :YScreenplayBase
{
    public YAudioSP(int selectId, HTimelineController timelineController) : base(selectId, timelineController)
    {
        id = 2;
        path = "Prefabs/YAudio/"+yPlanningTable.Instance.SelectTable[id][selectId];
    }
    public override void OnEnter()
    {
        base.OnEnter();
    }
        
    public override void LoadResourcesAndSetTimeline()
    {
        ////加载资源
        AudioClip audioClip = Resources.Load<AudioClip>(path);
        timelineController.ChangeSoundWithIndex(0, audioClip);
        //设置
        //timelineController.SetCharacter(go);
    }
}
