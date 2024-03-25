using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YIInteractiveGroup : MonoBehaviour
{
    public int InteractGroupID;
    public GameObject[] triggers; // 数组用于存储所有触发器（例如踏板）
    public int activatedTriggersCount = 0;
    
    [Header("是否离开要恢复")]
    public bool isRecover = false;
    [Header("是否只可开启一次（且不关闭）")]
    public bool isOnce = false;
    public bool hasEnter = false;//是否是第一次进入
    
    // Start is called before the first frame update
    public virtual void Start()
    {
        
        foreach (GameObject trigger in triggers)
        {
            // trigger.GetComponent<YTriggerUnit>().OnTriggerStateChanged+=CheckActivationStatus;
            trigger.GetComponent<YITriggerUnit>().OnTriggerStateChanged += CheckActivationStatus;
            trigger.GetComponent<YITriggerUnit>().OnEnterFieldStateChanged += EnterField;
        }
    }

    public virtual void CheckActivationStatus(bool activated)
    {
        if (activated)
        {
            activatedTriggersCount++;
        }
        else
        {
            activatedTriggersCount--;
        }

        if (isOnce && hasEnter)
        {
            return;
        }
        hasEnter = true;
        if (activatedTriggersCount >= triggers.Length) // 所有触发器都被激活
        {
            //ShowChest();
            //这里应该写一个通用的结果处理函数
            HAudioManager.Instance.Play("InteractionCommon", this.gameObject);
            SetResultOn();
        }
        else if(isRecover)
        {
            //HideChest();
            SetResultOff();
        }
    }

    
    //这里应该写一个通用的结果处理函数
    public virtual void SetResultOn()
    {
        
    }
    public virtual void SetResultOff()
    {
        
    }
    
    // public void EnterField(bool isEnter)
    public virtual void EnterField(bool isEnter, GameObject TriggergameObject,Transform showUIPlace)
    {
        if (isEnter)
        {
            if (isOnce && hasEnter)
            {
                return;
            }
            YTriggerEvents.RaiseOnShortcutKeyInteractionStateChanged(true, TriggergameObject,showUIPlace);
        }
        else
        {
            YTriggerEvents.RaiseOnShortcutKeyInteractionStateChanged(false, TriggergameObject,showUIPlace);
        }
    }

}
