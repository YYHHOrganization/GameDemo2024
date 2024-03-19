using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YLabelPanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/YLabelPanel";
    public YLabelPanel() : base(new UIType(path)){}
    
    public override void OnEnter()
    {
        uiTool.GetOrAddComponentInChilden<Button>("LabelButton").onClick.AddListener(()=>
        {
            //Debug.Log("点击了返回按钮");
            //YGameRoot.Instance.SceneSystem.SetScene(new YStartScene());
            Debug.Log("点击了Label按钮");
           
        });
        // uiTool.GetOrAddComponentInChilden<Button>("SettingButton").onClick.AddListener(()=>
        // {
        //     Debug.Log("点击了设置按钮");
        //     panelManager.Push(new SettingPanel());
        // });
        
    }
}