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

            other.GetComponent<YTriggerTagUnit>().OnEnterField(true);
            

            //为了测试我们直接假设点击了F键
            // YTriggerTagUnit triggerTagUnit = other.GetComponent<YTriggerTagUnit>();
            // triggerTagUnit.OnActive();
        }

        
        //Elevator
        if (other.CompareTag("Elevator"))
        {
            Debug.Log("电梯进入");
            //这里应该写一个通用的结果处理函数
            YInteractElevator interactElevator = other.GetComponentInParent<YInteractElevator>();
            interactElevator.PlayerEnterElevator(gameObject);
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
            
            other.GetComponent<YTriggerTagUnit>().OnEnterField(false);
            
            //YTriggerEvents.RaiseOnShortcutKeyInteractionStateChanged(false,other.gameObject);
            //为了测试我们直接假设点击了F键
            // YTriggerTagUnit triggerTagUnit = other.GetComponent<YTriggerTagUnit>();
            // triggerTagUnit.OnDeactive();
        }

        if (other.CompareTag("Elevator"))
        {
            Debug.Log("电梯离开");
            //这里应该写一个通用的结果处理函数
            YInteractElevator interactElevator = other.GetComponentInParent<YInteractElevator>();
            interactElevator.PlayerExitElevator();
        }
        
    }
}
