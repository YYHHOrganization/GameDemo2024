using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HRogueJudgeTriggerAndEvent : MonoBehaviour
{
    public string eventName;  //触发的事件的名字,后面如果频繁要使用这个脚本的话可以用if/else或者反射来做
    public string triggerTagName; //触发的标签名字

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(triggerTagName))
        {
            Destroy(other.gameObject);
            HRogueCameraManager.Instance.ShakeCamera(5f, 0.2f);
            YTriggerEvents.RaiseOnInterruptCombo(true);
        }
        Destroy(other, 2f);
    }
}
