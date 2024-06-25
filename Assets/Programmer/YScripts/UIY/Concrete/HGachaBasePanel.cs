using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HGachaBasePanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/HGachaBasePanel";
    private Button exitButton;
    public HGachaBasePanel() : base(new UIType(path))
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
            HPlayerSkillManager.instance.SetGachaPanelBeenPushed(false);
        });
    }

    public override void OnResume()
    {
        base.OnResume();
        YTriggerEvents.RaiseOnMouseLeftShoot(false);
    }
}
