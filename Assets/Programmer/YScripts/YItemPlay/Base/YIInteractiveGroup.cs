using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YIInteractiveGroup : MonoBehaviour
{
    public GameObject[] triggers; // 数组用于存储所有触发器（例如踏板）
    public int activatedTriggersCount = 0;
    
    [Header("是否离开要恢复")]
    public bool isRecover = false;
    
    // public event Action OnActivationStateChanged; // 定义激活状态改变事件
    
    // Start is called before the first frame update
    public virtual void Start()
    {
        
        foreach (GameObject trigger in triggers)
        {
            trigger.GetComponent<YTriggerUnit>().OnTriggerStateChanged+=CheckActivationStatus;
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

        if (activatedTriggersCount >= triggers.Length) // 所有触发器都被激活
        {
            //ShowChest();
            //这里应该写一个通用的结果处理函数
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
}
