using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YLevelPanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/YLevelPanel";
    private GameObject panelToAddUnitParent = null;
    public YLevelPanel() : base(new UIType(path)){}
    
    public override void OnEnter()
    {
        panelToAddUnitParent = uiTool.GetOrAddComponentInChilden<Transform>("PanelToAddUnit").gameObject;
        YLevelMain.Instance.InitLevelPanel(panelToAddUnitParent);
        
        //人为解锁第二个关卡
        //在实际游戏中玩家需要满足一定条件方可解锁关卡
        //此处仅作为演示
        //YLevelManager.SetLevels ("level1", true);
        
        uiTool.GetOrAddComponentInChilden<Button>("OkButton").onClick.AddListener(()=>
        {
            Debug.Log("点击了开始按钮");
             Pop();
             Push(new YChooseCharacterPanel());
        });
        
    }
}