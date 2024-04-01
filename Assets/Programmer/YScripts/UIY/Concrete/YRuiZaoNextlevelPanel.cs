using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YRuiZaoNextlevelPanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/RuiZaoLiuHePanel/YRuiZaoNextlevelPanel";
    YRuiZaoScripts yRuiZaoScripts;
    //bool finalWin;
    private Button NextLevelButton;
    public YRuiZaoNextlevelPanel(YRuiZaoScripts yRuiZaoScripts) : base(new UIType(path))
    {
        this.yRuiZaoScripts = yRuiZaoScripts;
        
    }
    
    public override void OnEnter()
    {
        NextLevelButton = uiTool.GetOrAddComponentInChilden<Button>("NextLevelButton");
        NextLevelButton.onClick.AddListener(()=>
        {
            Pop();  
            yRuiZaoScripts.BtnClickChangeLevel();
        });
        
        
        //ExitButton
        uiTool.GetOrAddComponentInChilden<Button>("ExitButton").onClick.AddListener(()=>
        {
            //弹出是否真的要退出挑战？
            HMessageShowMgr.Instance.ShowMessageWithActions("ConfirmExitRuizao", ExitRuizao, null,null);
        });
    }

    public void ExitRuizao()
    {
        Pop();
        Pop();
        Debug.Log("退出挑战");
        yRuiZaoScripts.ExitAndNoWin();
    }
}