using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YMainPanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/YMainPanel";
    public YMainPanel() : base(new UIType(path)){}
    
    public override void OnEnter()
    {
        // uiTool.GetOrAddComponentInChilden<Button>("BackButton").onClick.AddListener(()=>
        // {
        //     //Debug.Log("点击了返回按钮");
        //     //YGameRoot.Instance.SceneSystem.SetScene(new YStartScene());
        //     Pop();
        //     Push(new YChooseScreenplayPanel());
        //    
        // });
        // uiTool.GetOrAddComponentInChilden<Button>("SettingButton").onClick.AddListener(()=>
        // {
        //     Debug.Log("点击了设置按钮");
        //     panelManager.Push(new SettingPanel());
        // });
        
    }
}
