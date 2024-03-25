using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YInteractPortal : YIInteractiveGroup
{

    public void Start()
    {
        base.Start();
    }
    
    public override void SetResultOn()
    {
        Debug.Log("传送门开启交互");
        
        //如果是靠近门打开 只会有一个trigger
        YTriggerEvents.RaiseOnShortcutKeyInteractionStateChanged(false, triggers[0],null);
        
    }
    public override void SetResultOff()
    {
        
    }
    
    public override void EnterField(bool isEnter, GameObject TriggergameObject,Transform showUIPlace)
    {
        base.EnterField(isEnter, TriggergameObject,showUIPlace);
        if (isEnter)
        {
            Debug.Log("进入传送门区域");
        }
        else
        {
            Debug.Log("离开传送门区域");
        }
    
    }
    
}
