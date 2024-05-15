
using System;
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
        //传送角色
        YPlayModeController.Instance.SetRogueCharacterPlace(transferPlace);
        
        //传送后的效果
        //1.传送门ui消失
        getUI.gameObject.SetActive(false);
        OnPlayerPortal?.Invoke();
        
    }
    
}
