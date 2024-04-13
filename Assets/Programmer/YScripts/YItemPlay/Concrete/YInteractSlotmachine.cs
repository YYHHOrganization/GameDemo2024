using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YInteractSlotmachine : YIInteractiveGroup
{
    GameObject slotmachine;
    public void Start()
    {
        base.Start();
        
    }
    
    public override void SetResultOn()
    {
        YTriggerEvents.RaiseOnShortcutKeyInteractionStateChanged(false, triggers[0],null);//如果是靠近宝箱打开 只会有一个trigger
        Debug.Log("开启");
        slotmachine.GetComponent<HSlotMachineBase >().GiveOutSomethingAndPlay();
    }

    
    public override void SetResultOff()
    {
        
    }
    
    public override void EnterField(bool isEnter, GameObject TriggergameObject,Transform showUIPlace)
    {
        if(slotmachine == null)
            slotmachine = transform.Find("main/SlotMachineNew").gameObject;
        base.EnterField(isEnter, TriggergameObject,showUIPlace);
        if (isEnter)
        {
            Debug.Log("进入区域");
        }
        else
        {
            Debug.Log("离开区域");
        }
    
    }
    
}
