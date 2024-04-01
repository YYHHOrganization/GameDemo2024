using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class YLossAndNextLevelPanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/YLossAndNextLevelPanel";
    private string TextStr="挑战失败";

    public YLossAndNextLevelPanel() : base(new UIType(path))
    {
        TextStr = "挑战失败";
    }

    public YLossAndNextLevelPanel(string Textstr) : base(new UIType(path))
    {
        TextStr = Textstr;
    }
    
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
            //Pop();
            RemoveSelfPanel();
            Push(new YChooseCharacterPanel());
            //变回全屏
            YPlayModeController.Instance.SetCameraLayout(0);
        });
        //ChooseLevelButton
        uiTool.GetOrAddComponentInChilden<Button>("ChooseLevelButton").onClick.AddListener(()=>
        {
            Pop();
            Push(new YLevelPanel(0));
        });
        TMP_Text LossText = uiTool.GetOrAddComponentInChilden<TMP_Text>("LossText");
        LossText.text = TextStr;
        
    }
}