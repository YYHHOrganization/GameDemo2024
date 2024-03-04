using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YScreenplayBase
{
    //剧本元组的id
    public int id;
    //剧本 选择了哪个元组
    public int selectId;
    //路径
    public string path;
    protected HTimelineController timelineController;
    public YScreenplayBase(int selectId,HTimelineController timelineController)
    {
        this.selectId = selectId;
        this.timelineController = timelineController;
    }
    //进入
    public virtual void OnEnter()
    {
        LoadResourcesAndSetTimeline();
    }

    public virtual void LoadResourcesAndSetTimeline()
    {
        
    }
}
