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
            
            Push(new YLevelPanel(1));//演绎模式
            // Push(new YChooseScreenplayPanel());
        });
        //游玩模式
        uiTool.GetOrAddComponentInChilden<Button>("PlayButton").onClick.AddListener(()=>
        {
            Debug.Log("点击了Play按钮");
            Pop();
            //todo:加载一个场景，放入一个荧妹当主角，比如一个房间什么的，现在还没到选关卡这个步骤。
            Push(new YLevelPanel(0));//游玩模式
            // Push(new YChooseCharacterPanel());
        });
        uiTool.GetOrAddComponentInChilden<Button>("SettingButton").onClick.AddListener(()=>
        {
            //Debug.Log("点击了设置按钮");
            Push(new SettingPanel());
        });
    }
}
