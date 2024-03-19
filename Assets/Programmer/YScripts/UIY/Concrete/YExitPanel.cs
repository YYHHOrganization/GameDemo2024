using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YExitPanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/YExitPanel";

    public YExitPanel() : base(new UIType(path))
    {
        
    }

    
    public override void OnEnter()
    {
        //存储鼠标状态
        bool isMouseLocked = Cursor.lockState == CursorLockMode.Locked;
        //锁住鼠标
        YTriggerEvents.RaiseOnMouseLockStateChanged(false);
        
        //此时应该禁止再次点击退出快捷键
        YTriggerEvents.RaiseOnShortcutKeyEsc(false);
       
        
        uiTool.GetOrAddComponentInChilden<Button>("PlayAgainButton").onClick.AddListener(()=>
        {
            //游戏重新开始：场景重新load
            YGameRoot.Instance.PlayAgain();
        });
        
        uiTool.GetOrAddComponentInChilden<Button>("ExitGameButton").onClick.AddListener(()=>
        {
            //退出游戏
            YGameRoot.Instance.QuitGame();
        });
        
        uiTool.GetOrAddComponentInChilden<Button>("ReturnGameButton").onClick.AddListener(()=>
        {
            //回到鼠标状态
            YTriggerEvents.RaiseOnMouseLockStateChanged(isMouseLocked);
            YTriggerEvents.RaiseOnShortcutKeyEsc(true);
            Pop();
        });
       
        
    }
    
    
}
