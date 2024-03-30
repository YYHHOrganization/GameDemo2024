using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YLossAndNextLevelPanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/YLossAndNextLevelPanel";
    public YLossAndNextLevelPanel() : base(new UIType(path)) { }
    
    public override void OnEnter()
    {
        YTriggerEvents.RaiseOnMouseLockStateChanged(false);
        //人为解锁第二个关卡
        //在实际游戏中玩家需要满足一定条件方可解锁关卡
        //此处仅作为演示
        //YLevelManager.SetLevels ("level1", true);
        
        uiTool.GetOrAddComponentInChilden<Button>("AgainButton").onClick.AddListener(()=>
        {
            YLevelManager.LoadAndBeginLevel();
            Pop();
            Push(new YChooseCharacterPanel());
        });
        //ChooseLevelButton
        uiTool.GetOrAddComponentInChilden<Button>("ChooseLevelButton").onClick.AddListener(()=>
        {
            Pop();
            Push(new YLevelPanel(0));
        });
        
    }
}