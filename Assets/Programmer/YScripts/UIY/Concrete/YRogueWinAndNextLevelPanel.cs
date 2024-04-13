using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class YRogueWinAndNextLevelPanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/YRogueWinAndNextLevelPanel";
    string levelName = null;
    private int RogueLevel;
    public YRogueWinAndNextLevelPanel(string levelName) : base(new UIType(path))
    {
        this.levelName = levelName;
        RogueLevel = YRogueDungeonManager.Instance.GetRogueLevel();
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
            YRogueDungeonManager.Instance.EnterNewLevel();
            RemoveSelfPanel();
            Push(new YRogueLoadPanel());
            
        });
        uiTool.GetOrAddComponentInChilden<TMP_Text>("LevelStr").text = (RogueLevel+1).ToString();
        
    }
}