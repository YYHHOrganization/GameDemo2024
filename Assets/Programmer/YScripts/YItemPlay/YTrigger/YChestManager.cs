using UnityEngine;

public class YChestManager : MonoBehaviour
{
    private static int activatedTriggersCount = 0;
    public GameObject chest;//后面可以改为动态加载
    private void OnEnable()
    {
        YTriggerEvents.OnTriggerStateChanged += HandleTriggerStateChanged;//含义是当触发器状态改变时，调用HandleTriggerStateChanged方法
    }

    private void OnDisable()
    {
        YTriggerEvents.OnTriggerStateChanged -= HandleTriggerStateChanged;
    }

    private void HandleTriggerStateChanged(object sender, YTriggerEventArgs e)//sender是触发事件的对象，e是事件的参数
    {
        if (e.activated)
        {
            activatedTriggersCount++;
        }
        else
        {
            activatedTriggersCount--;
        }

        if (activatedTriggersCount >= 2) // 两个机关都被激活
        {
            ShowChest();
        }
        else
        {
            HideChest();
        }
        // if (e.activated)
        // {
        //     triggersActivated++;
        //     // 处理触发器被激活的逻辑，比如显示宝箱
        //     //ShowChest();
        //     if (triggersActivated == 2)
        //     {
        //         // 处理触发器被激活的逻辑，比如显示宝箱
        //         ShowChest();
        //     }
        // }
        // else
        // {
        //     triggersActivated--;
        //     // 处理触发器停止激活的逻辑，比如隐藏宝箱
        //     //HideChest();
        // }
    }

    private void ShowChest()
    {
        // 显示宝箱的逻辑
        chest.SetActive(true);
    }

    private void HideChest()
    {
        // 隐藏宝箱的逻辑
    }
}