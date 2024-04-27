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

    private Transform NextlevelStr;
    private Transform level0Str;
    public YRogueWinAndNextLevelPanel(string levelName) : base(new UIType(path))
    {
        this.levelName = levelName;
        RogueLevel = YRogueDungeonManager.Instance.GetRogueLevel();
    }
    public YRogueWinAndNextLevelPanel() : base(new UIType(path))
    {
        RogueLevel = YRogueDungeonManager.Instance.GetRogueLevel();
    }
    
    public override void OnEnter()
    {
        YTriggerEvents.RaiseOnMouseLockStateChanged(false);
        YTriggerEvents.RaiseOnMouseLeftShoot(false);
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
        
        NextlevelStr = uiTool.GetOrAddComponentInChilden<Transform>("NextlevelStr");
        level0Str = uiTool.GetOrAddComponentInChilden<Transform>("level0Str");
        if(RogueLevel == -1)
        {
            NextlevelStr.gameObject.SetActive(false);
            level0Str.gameObject.SetActive(true);
        }
        else
        {
            NextlevelStr.gameObject.SetActive(true);
            level0Str.gameObject.SetActive(false);
        }
    }
}