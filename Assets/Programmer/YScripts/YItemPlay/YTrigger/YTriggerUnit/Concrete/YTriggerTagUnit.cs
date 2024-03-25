using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YTriggerTagUnit : MonoBehaviour, YITriggerUnit
{
    public Transform showUI;
    public event Action<bool> OnTriggerStateChanged;
    public event Action<bool,GameObject,Transform> OnEnterFieldStateChanged;
    
    //事件激活
    public void OnActive()
    {
        OnTriggerStateChanged?.Invoke(true);
    }
    //事件关闭
    public void OnDeactive()
    {
        OnTriggerStateChanged?.Invoke(false);
    }
    //进入触发区域
    public void OnEnterField(bool isEnter)
    {
        OnEnterFieldStateChanged?.Invoke(isEnter,gameObject,showUI);
        //Debug.Log("OnEnterField！！！");
        // YTriggerEvents.RaiseOnShortcutKeyInteractionStateChanged(true, gameObject);
    }   

}
