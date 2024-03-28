using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YLevelPanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/YLevelPanel";
    private GameObject panelToAddUnitParent = null;
    Image ShowLevelScenePanelImage;

    private int PlayModeIndex;

    public YLevelPanel(int modeIndex) : base(new UIType(path))
    {
        PlayModeIndex = modeIndex;
    }
    
    public override void OnEnter()
    {
        ShowLevelScenePanelImage = uiTool.GetOrAddComponentInChilden<Image>("ShowLevelScenePanelImage");
        panelToAddUnitParent = uiTool.GetOrAddComponentInChilden<Transform>("PanelToAddUnit").gameObject;
        YLevelMain.Instance.InitLevelPanel(panelToAddUnitParent,this);
        
        //人为解锁第二个关卡
        //在实际游戏中玩家需要满足一定条件方可解锁关卡
        //此处仅作为演示
        //YLevelManager.SetLevels ("level1", true);
        
        uiTool.GetOrAddComponentInChilden<Button>("OkButton").onClick.AddListener(()=>
        {
            YLevelManager.LoadAndBeginLevel();
            Debug.Log("点击了开始按钮");
             Pop();

             if (PlayModeIndex == 0)
             {
                 Push(new YChooseCharacterPanel());
             }
             else
             {
                 Push(new YChooseScreenplayPanel());
             }
        });
        
    }

    public void SetCurLevel(int index)
    {
        string namestr = "level"+index;
        Texture2D tex2D;
        tex2D=Resources.Load("Prefabs/UI/YImage/ShowLevelScenePanelImage/"+namestr) as Texture2D;
        Sprite sprite=Sprite.Create(tex2D,new Rect(0,0,tex2D.width,tex2D.height),new Vector2(0.5F,0.5F));
        ShowLevelScenePanelImage.sprite=sprite;
    }
}