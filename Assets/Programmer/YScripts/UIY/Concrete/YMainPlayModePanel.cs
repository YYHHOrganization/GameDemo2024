using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YMainPlayModePanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/YMainPlayModePanel";

    public YMainPlayModePanel() : base(new UIType(path))
    {
        
    }

    
    public override void OnEnter()
    {
        //进入这个panel 代表已经召唤完伙伴了 进入召唤完的panel
        YPlayModeController.Instance.IsAfterEnterPuppetPanel = true;
        
        YTriggerEvents.RaiseOnMouseLockStateChanged(true);
        YTriggerEvents.RaiseOnShortcutKeySplitScreenStateChanged(true);
        uiTool.GetOrAddComponentInChilden<Button>("SetTwoCamerasEachHalfButton").onClick.AddListener(()=>
        {
            //设置为半屏
            YPlayModeController.Instance.SetCameraLayout(2);
           
        });
        uiTool.GetOrAddComponentInChilden<Button>("SetPuppetCameraLittleButton").onClick.AddListener(()=>
        {
            //设置为小屏
            YPlayModeController.Instance.SetCameraLayout(1);
           
        });
        uiTool.GetOrAddComponentInChilden<Button>("SetPlayerCameraWholeScreenButton").onClick.AddListener(()=>
        {
            //设置为全屏
            YPlayModeController.Instance.SetCameraLayout(0);
           
        });
        uiTool.GetOrAddComponentInChilden<Button>("ExitButton").onClick.AddListener(()=>
        {
            //显示出是否退出面板
            Push(new YExitPanel());
           
        });
       
        
    }
    //如果输入F 也认为点击了GetPuppetButton
    
}
