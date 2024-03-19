using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YTriggerUnit : MonoBehaviour, YITriggerUnit
{
    private int activatedTriggers = 0;//比如人数
    public bool activated=false;
    public event Action<bool> OnTriggerStateChanged; // 定义状态改变事件
    public event Action<GameObject> OnEnterFieldStateChanged; // 定义进入触发区域事件
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")||other.CompareTag("Puppet"))
        {
            //Debug.Log("OnTriggerEnter！！！");
            activatedTriggers++;
            if (IsActivated())
            {
                if(activated==false)
                {
                    activated = true;
                    //Debug.Log("OnTriggerEnter！！！IsActivated");
                    // YTriggerEvents.RaiseOnTriggerStateChanged(true);
                    OnTriggerStateChanged?.Invoke(true);
                }
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("OnTriggerExit！！！");
        if (other.CompareTag("Player")||other.CompareTag("Puppet"))
        {
            activatedTriggers--;
            if (!IsActivated())
            {
                //Debug.Log("OnTriggerExit！！！IsActivated");
                // YTriggerEvents.RaiseOnTriggerStateChanged(false);
                OnTriggerStateChanged?.Invoke(false);
                activated = false;
            }
        }
    }
    
    private bool IsActivated()
    {
        return activatedTriggers > 0;
    }
}