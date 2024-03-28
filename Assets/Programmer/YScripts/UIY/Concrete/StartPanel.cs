using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/StartPanel";
    public StartPanel() : base(new UIType(path)){}
    
    public override void OnEnter()
    {
        //演绎模式
        uiTool.GetOrAddComponentInChilden<Button>("StartButton").onClick.AddListener(()=>
        {
            //Debug.Log("点击了开始按钮");
            //YGameRoot.Instance.SceneSystem.SetScene(new YMainScene());
            Pop();
            //Push(new YMainPanel());
            Push(new YChooseScreenplayPanel());
        });
        //游玩模式
        uiTool.GetOrAddComponentInChilden<Button>("PlayButton").onClick.AddListener(()=>
        {
            Debug.Log("点击了Play按钮");
            Pop();
            Push(new YLevelPanel());
            // Push(new YChooseCharacterPanel());
        });
        uiTool.GetOrAddComponentInChilden<Button>("SettingButton").onClick.AddListener(()=>
        {
            //Debug.Log("点击了设置按钮");
            Push(new SettingPanel());
        });
    }
}
