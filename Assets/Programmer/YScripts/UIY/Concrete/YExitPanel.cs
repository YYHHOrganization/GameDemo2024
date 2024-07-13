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
       
        uiTool.GetOrAddComponentInChilden<Button>("SettingButton").onClick.AddListener(()=>
        {
            //Debug.Log("点击了设置按钮");
            Push(new SettingPanel());
        });
        
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
        
        uiTool.GetOrAddComponentInChilden<Button>("FangkasiButton").onClick.AddListener(TeleportToCurrentRoom);
    }

    private void TeleportToCurrentRoom()
    {
        GameObject player = YPlayModeController.Instance.curCharacter;
        if (player == null)
        {
            return;
        }
        player.GetComponent<CharacterController>().enabled = false;
        GameObject lastRoom = YRogue_RoomAndItemManager.Instance.LastRoom;
        if (lastRoom)
        {
            player.transform.position = lastRoom.transform.position;
        }
        else
        {
            //注：双子塔卡死脱离的时候，雾效还在，但是应该不影响，那个地图不太会卡死，先这样吧
            GameObject curRoom = YRogue_RoomAndItemManager.Instance.currentRoom;
            if (curRoom)
            {
                player.transform.position = curRoom.transform.position;
            }
        }
        player.GetComponent<CharacterController>().enabled = true;
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
