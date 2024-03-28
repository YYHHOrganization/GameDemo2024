using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class YInteractPortal : YIInteractiveGroup
{
    private string needId = "20000009";
    private int needCount = 3;
    HItemShowPanel panel = new HItemShowPanel();
    private bool isPushed;
    private int actualCount;
    public void Start()
    {
        base.Start();
        //todo:后面记得去掉
        if (debugMode)
        {
            actualCount = 3;
        }
    }
    
    public bool debugMode = false;

    private void Update()
    {
        
    }

    public override void SetResultOn()
    {
        Debug.Log("传送门开启交互");
        
        //如果是靠近门打开 只会有一个trigger
        YTriggerEvents.RaiseOnShortcutKeyInteractionStateChanged(false, triggers[0],null);
        StartCoroutine(ShowNeedItemPanel());

    }

    IEnumerator ShowNeedItemPanel()
    {
        YGameRoot.Instance.Push(panel);
        YTriggerEvents.RaiseOnMouseLockStateChanged(false);
        YPlayModeController.Instance.LockPlayerInput(true);
        
        isPushed = true;
        panel.SetMiddlePanelActive(false);
        panel.SetLeftScrollPanelActive(false);
        panel.SetGiveOutPanelActive(true);
        
        var item = yPlanningTable.Instance.worldItems[needId];
        AsyncOperationHandle<Sprite> handle =
            Addressables.LoadAssetAsync<Sprite>(item.UIIconLink);
        yield return handle;
        if (!debugMode)
        {
            actualCount = HItemCounter.Instance.CheckCountWithItemId(needId);
        }
        panel.ShowGiveOutItems(handle.Result, needCount, actualCount, this);
    }


    public bool CheckCountIsRightOrNot()
    {
        return (needCount==actualCount);
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
            HAudioManager.Instance.Play("PortalAudio", gameObject);
        }
        else
        {
            StopAllCoroutines();
            //HAudioManager.Instance.Stop(gameObject);
        }
    
    }
    
}
