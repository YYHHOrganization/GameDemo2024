using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class YInteractPortalInRogue : YIInteractiveGroup
{
    public GameObject MeshToDissolve;
    public GameObject InsidePortalToSHow;
    
    public void Start()
    {
        base.Start();
        InsidePortalToSHow.SetActive(false);
        ShowUp();  
    }
    
    public override void SetResultOn()
    {
        Debug.Log("传送门开启交互");
        
        //如果是靠近门打开 只会有一个trigger
        YTriggerEvents.RaiseOnShortcutKeyInteractionStateChanged(false, triggers[0],null);
        //出现下一关字样
        YGameRoot.Instance.Push(new YRogueWinAndNextLevelPanel(""));
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

    public void ShowUp()
    {
        DissolvingControllery dissolving = gameObject.GetComponent<DissolvingControllery>();
        dissolving.SetBeginAndEndAndMaterialsPropAndBeginDissolve(MeshToDissolve,0.5f,1,0);
        
        
        StartCoroutine(ShowPortalInside());
        
    }

    private IEnumerator ShowPortalInside()
    {
        yield return new WaitForSeconds(1.5f);
        InsidePortalToSHow.SetActive(true);
        InsidePortalToSHow.transform.DOScale(Vector3.one, 1f);
    }
}
