
using System;
using DG.Tweening;
using UnityEngine;

public class YPortalToSomeWhere :YRogue_TriggerGame
{
    //Start
    private void Start()
    {
        base.Start();
    }
    public Action OnPlayerPortal;
    //StartInteract()
    protected override void StartInteract()
    {
        //传送
        //1.找到传送的地点
        //2.传送
        //3.传送后的效果
        Debug.Log("传送回相应位置");
        Vector3 transferPlace = YRogueDungeonManager.Instance.GetTransferPlace();
        if (transferPlace == null)
        {
            Debug.LogError("没有找到传送地点");
            return;
        }
        
        HMessageShowMgr.Instance.ShowMessage("ROGUE_TUTORIAL_SCENE_EXIT" );
        DOVirtual.DelayedCall(0.5f, () =>
        {
            //传送角色
            YPlayModeController.Instance.SetRogueCharacterPlace(transferPlace);
            Debug.Log(transferPlace+"传送回相应位置");
        
            //传送后的效果
            //1.传送门ui消失
            
            OnPlayerPortal?.Invoke();
        });
        
        getUI.gameObject.SetActive(false);
        
    }
    
}
