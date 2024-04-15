using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class YRuiZaoWinPanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/RuiZaoLiuHePanel/YRuiZaoWinPanel";
    YRuiZaoScripts yRuiZaoScripts;
    private Button NextLevelButton;
    public YRuiZaoWinPanel(YRuiZaoScripts yRuiZaoScripts) : base(new UIType(path))
    {
        this.yRuiZaoScripts = yRuiZaoScripts;
        
    }
    
    public override void OnEnter()
    {
        NextLevelButton = uiTool.GetOrAddComponentInChilden<Button>("WinButton");
        NextLevelButton.onClick.AddListener(()=>
        {
            RemoveSelfPanel();
            Pop();

            if (YPlayModeController.Instance.isRogue==false)
            {
                bool isAfterEnterPuppetPanel = YPlayModeController.Instance.IsAfterEnterPuppetPanel;
                if (isAfterEnterPuppetPanel)
                {
                    Push(new YMainPlayModePanel());
                }
                else
                {
                    Push(new YMainPlayModeOriginPanel());
                }
            }

            yRuiZaoScripts.WinAndExit();
            Debug.Log("赢了！！！");
        });
        
        
    }
}