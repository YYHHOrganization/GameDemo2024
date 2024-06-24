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

    private int timeScale = 1;
    public override void OnEnter()
    {
        //存储鼠标状态
        bool isMouseLocked = Cursor.lockState == CursorLockMode.Locked;
        
        //暂停游戏，防止被怪打死
        timeScale = (int)Time.timeScale;
        Time.timeScale = 0.01f; //如果是0的话，重新恢复会导致人抖动一下，感觉跟状态机有关，懒得修了，改成0.01
        
        //锁住鼠标
        YTriggerEvents.RaiseOnMouseLockStateChanged(false);
        YTriggerEvents.RaiseOnMouseLeftShoot(false);
        
        //此时应该禁止再次点击退出快捷键
        YTriggerEvents.RaiseOnShortcutKeyEsc(false);
       
        
        uiTool.GetOrAddComponentInChilden<Button>("PlayAgainButton").onClick.AddListener(()=>
        {
            //游戏重新开始：场景重新load
            YGameRoot.Instance.PlayAgain();
        });
        
        uiTool.GetOrAddComponentInChilden<Button>("ExitGameButton").onClick.AddListener(()=>
        {
            //退出游戏,二次确认窗口
            HMessageShowMgr.Instance.ShowMessageWithActions("ConfirmExitGame", QuitGame, ResumeGame, ResumeGame);
        });
        
        uiTool.GetOrAddComponentInChilden<Button>("ReturnGameButton").onClick.AddListener(()=>
        {
            ResumeGame();
        });
    }

    private void ResumeGame()
    {
        bool isMouseLocked = Cursor.lockState == CursorLockMode.Locked;
        //回到鼠标状态
        YTriggerEvents.RaiseOnMouseLockStateChanged(isMouseLocked);
        YTriggerEvents.RaiseOnShortcutKeyEsc(true);
        YTriggerEvents.RaiseOnMouseLeftShoot(true);
        Time.timeScale = timeScale;
        Pop();
    }

    private void QuitGame()
    {
        YGameRoot.Instance.QuitGame();
    }
    
    
}
