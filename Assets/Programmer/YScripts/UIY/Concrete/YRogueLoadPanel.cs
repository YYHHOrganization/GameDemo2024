using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class YRogueLoadPanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/YRogueLoadPanel";
    Button enterGameButton = null;
    
    Slider slider = null;
    YSliderChange sliderChange = null;
    
    public float loadFakeTime = 1f;
    public float loadFinalTime = 0.1f;
    private int RogueLevel;
    public YRogueLoadPanel() : base(new UIType(path)){}
    
    public override void OnEnter()
    {
        RogueLevel = YRogueDungeonManager.Instance.GetRogueLevel();
        // YTriggerEvents.OnLoadResourceStateChanged += Loadend;
        
        //2s后出现按钮EnterGameButton
        //Invoke("ShowEnterGameButton",2f);
        
        //加载进度条
        // uiTool.GetOrAddComponentInChilden<Slider>("LoadSlider")
        //     .GetComponent<YSliderChange>().SetSliderOn(0,0.9f,0.2f);
        slider = uiTool.GetOrAddComponentInChilden<Slider>("LoadSlider");
        sliderChange = slider.GetComponent<YSliderChange>();
        sliderChange.SetSliderOn(0,1f,loadFakeTime,slider,this);  
        uiTool.GetOrAddComponentInChilden<TMP_Text>("LevelStr").text = (RogueLevel+1).ToString();
    }
    public override void OnExit()
    {
        base.OnExit();
        // YTriggerEvents.OnLoadResourceStateChanged -= Loadend;
    }
    
    
}