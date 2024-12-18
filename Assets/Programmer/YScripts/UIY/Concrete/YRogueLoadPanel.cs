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
    
    public float loadFakeTime = 4f;
    public float loadFinalTime = 0.1f;
    private int RogueLevel;
    public YRogueLoadPanel() : base(new UIType(path)){}
    
    public override void OnEnter()
    {
        YTriggerEvents.RaiseOnMouseLockStateChanged(false);
        
        RogueLevel = YRogueDungeonManager.Instance.GetRogueLevel();
        // YTriggerEvents.OnLoadResourceStateChanged += Loadend;
        
        //2s后出现按钮EnterGameButton
        //Invoke("ShowEnterGameButton",2f);
        
        //加载进度条
        // uiTool.GetOrAddComponentInChilden<Slider>("LoadSlider")
        //     .GetComponent<YSliderChange>().SetSliderOn(0,0.9f,0.2f);
        slider = uiTool.GetOrAddComponentInChilden<Slider>("LoadSlider");
        sliderChange = slider.GetComponent<YSliderChange>();
        loadFakeTime = YRogueDungeonManager.Instance.GetLoadFakeTime();
        sliderChange.SetSliderOn(0,1f,loadFakeTime,slider,this);  
        uiTool.GetOrAddComponentInChilden<TMP_Text>("LevelStr").text = (RogueLevel+1).ToString();
        
        Transform ySortScenicSpotPlace = uiTool.GetOrAddComponentInChilden<Transform>("scenicSpotPlace");
        YSortScenicSpotPlace sortScenicSpotPlace = ySortScenicSpotPlace.GetComponent<YSortScenicSpotPlace>();
        Transform ImageCharacter = uiTool.GetOrAddComponentInChilden<Transform>("ImageCharacter");
        sortScenicSpotPlace.ShowScenicSpotPlace(RogueLevel, ImageCharacter,loadFakeTime);
        
        string curScenicSpotPlaceName = sortScenicSpotPlace.GetScenicSpotPlaceName(RogueLevel);
        uiTool.GetOrAddComponentInChilden<TMP_Text>("ScenicSpotPlaceName").text = curScenicSpotPlaceName;
        
    }
    public override void OnExit()
    {
        base.OnExit();
        YTriggerEvents.RaiseOnMouseLockStateChanged(true);
        YTriggerEvents.RaiseOnMouseLeftShoot(true);
        YTriggerEvents.RaiseOnLoadEndAndBeginPlay(true);
    }
    
    
}