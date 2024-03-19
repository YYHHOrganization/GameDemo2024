using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YPlayerTriggerController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TriggerToLabel"))
        {
            Debug.Log("OnTriggerEnter！！！"+other.name);
            //这时候 应该显示label表示可以交互 比如有F键的提示
            //然后应该开启点击F键可以交互的事件
            //然后监听是否点击F  如果点击了F  就应该调用交互事件 这里要传入other 获取他上面的东西

            other.GetComponent<YTriggerTagUnit>().OnEnterField();
            

            //为了测试我们直接假设点击了F键
            // YTriggerTagUnit triggerTagUnit = other.GetComponent<YTriggerTagUnit>();
            // triggerTagUnit.OnActive();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TriggerToLabel"))
        {
            //Debug.Log("OnTriggerExit！！！"+other.name);
            //这时候 应该隐藏label表示可以交互 比如有F键的提示
            //然后应该关闭点击F键可以交互的事件
            //然后监听是否点击F  如果点击了F  就应该调用交互事件 这里要传入other 获取他上面的东西
            YTriggerEvents.RaiseOnShortcutKeyInteractionStateChanged(false,other.gameObject);
            //为了测试我们直接假设点击了F键
            // YTriggerTagUnit triggerTagUnit = other.GetComponent<YTriggerTagUnit>();
            // triggerTagUnit.OnDeactive();
        }
    }
}
