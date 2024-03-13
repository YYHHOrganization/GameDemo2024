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
            YPlayModeController.Instance.DetectViewOnOrOff();
        });
        
    }
    //如果输入F 也认为点击了GetPuppetButton
    
}
