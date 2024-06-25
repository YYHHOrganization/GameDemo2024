using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GMPanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/GMPanel";
    private Button exitButton;
    public GMPanel() : base(new UIType(path))
    {
        
    }

    public override void OnEnter()
    {
        HRoguePlayerAttributeAndItemManager.Instance.IsUsingGMPanel = true;
        YTriggerEvents.RaiseOnMouseLeftShoot(false);
        exitButton = uiTool.GetOrAddComponentInChilden<Button>("ExitButton");
        //呼出鼠标
        YTriggerEvents.RaiseOnMouseLockStateChanged(false);
        YPlayModeController.Instance.LockPlayerInput(true);
        exitButton.onClick.AddListener(() =>
        {
            YTriggerEvents.RaiseOnMouseLeftShoot(true);
            YPlayModeController.Instance.LockPlayerInput(false);
            HRoguePlayerAttributeAndItemManager.Instance.IsUsingGMPanel = false;
            RemoveSelfPanel();
            //HPlayerSkillManager.instance.SetBagBeenPushed(false);
        });
    }

    public override void OnResume()
    {
        HRoguePlayerAttributeAndItemManager.Instance.IsUsingGMPanel = true;
        //Debug.Log("OnResume, HBagPanel");
        base.OnResume();
        YTriggerEvents.RaiseOnMouseLeftShoot(false);
    }
}
