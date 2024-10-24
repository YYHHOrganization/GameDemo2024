using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HBagPanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/HBagPanel";
    private Button exitButton;
    public HBagPanel() : base(new UIType(path))
    {
        
    }

    public override void OnEnter()
    {
        YTriggerEvents.RaiseOnMouseLeftShoot(false);
        exitButton = uiTool.GetOrAddComponentInChilden<Button>("ExitButton");
        //呼出鼠标
        YTriggerEvents.RaiseOnMouseLockStateChanged(false);
        YPlayModeController.Instance.LockPlayerInput(false);
        exitButton.onClick.AddListener(() =>
        {
            YTriggerEvents.RaiseOnMouseLeftShoot(true);
            RemoveSelfPanel();
            HPlayerSkillManager.instance.SetBagBeenPushed(false);
        });
    }

    public override void OnResume()
    {
        //Debug.Log("OnResume, HBagPanel");
        base.OnResume();
        YTriggerEvents.RaiseOnMouseLeftShoot(false);
    }
}
