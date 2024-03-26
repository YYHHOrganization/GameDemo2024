using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YWinAndNextLevelPanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/YWinAndNextLevelPanel";
    string levelName = null;
    public YWinAndNextLevelPanel(string levelName) : base(new UIType(path))
    {
        this.levelName = levelName;
    }
    
    public override void OnEnter()
    {
        YTriggerEvents.RaiseOnMouseLockStateChanged(false);
        //人为解锁第二个关卡
        //在实际游戏中玩家需要满足一定条件方可解锁关卡
        //此处仅作为演示
        //YLevelManager.SetLevels ("level1", true);
        
        uiTool.GetOrAddComponentInChilden<Button>("NextLevelButton").onClick.AddListener(()=>
        {
            Debug.Log("点击了下一关按钮"+levelName);
            YLevelManager.LoadAndBeginLevel(levelName);
            Pop();
            Push(new YChooseCharacterPanel());
        });
        
    }
}