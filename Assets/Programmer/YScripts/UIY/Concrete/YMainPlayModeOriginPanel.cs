using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YMainPlayModeOriginPanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/YMainPlayModeOriginPanel";
    public YMainPlayModeOriginPanel() : base(new UIType(path))
    {
        
    }
    public YMainPlayModeOriginPanel(bool GetPu) : base(new UIType(path))
    {
        //uiTool.DisableComponentInChilden<Button>("GetPuppetButton");
    }
    
    public override void OnEnter()
    {
        //鼠标锁定
        YTriggerEvents.RaiseOnMouseLockStateChanged(true);
        //可以通过快捷键呼出人偶
        YTriggerEvents.RaiseOnPuppetShortCutStateChanged(true);
        
        uiTool.GetOrAddComponentInChilden<Button>("GetPuppetButton").onClick.AddListener(()=>
        {
            //Debug.Log("点击了返回按钮");
            //YGameRoot.Instance.SceneSystem.SetScene(new YStartScene());
            Pop();
            Push(new YChooseScreenplayInPlayModePanel());
            //设置为半屏
            YPlayModeController.Instance.SetCameraLayout(2);
           
        });
        uiTool.GetOrAddComponentInChilden<Button>("DetectButton").onClick.AddListener(()=>
        {
            //Debug.Log("点击了返回按钮");
            //YPlayModeController.Instance.SetCameraLayout(2);
            //YPlayModeController.Instance.DetectViewOnOrOff();
            HPlayerSkillManager.instance.SkillScanningTerrian();
            //uiTool.GetOrAddComponentInChilden<Button>("DetectButton").interactable = false;
        });
        
    }
    //如果输入F 也认为点击了GetPuppetButton
    public override void OnPause()
    {
        base.OnPause();
        YTriggerEvents.RaiseOnPuppetShortCutStateChanged(false);
    }
    public override void OnResume()
    {
        base.OnResume();
        YTriggerEvents.RaiseOnPuppetShortCutStateChanged(true);
    }
    public override void OnExit()
    {
        base.OnExit();
        YTriggerEvents.RaiseOnPuppetShortCutStateChanged(false);
    }
}
