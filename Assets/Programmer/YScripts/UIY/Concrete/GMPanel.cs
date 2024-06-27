using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GMPanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/GMPanel";
    private Button exitButton;
    public GMPanel() : base(new UIType(path))
    {
        
    }

    private float timeScale = 1f;
    private Tween tween;
    public override void OnEnter()
    {
        HRoguePlayerAttributeAndItemManager.Instance.IsUsingGMPanel = true;
        timeScale = Time.timeScale;
        tween = DOVirtual.DelayedCall(2f, () => Time.timeScale = 0.01f);
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
            tween.Kill();
            Time.timeScale = timeScale;
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
