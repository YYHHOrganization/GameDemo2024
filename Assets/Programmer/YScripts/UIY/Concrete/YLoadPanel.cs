using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class YLoadPanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/YLoadPanel";
    Button enterGameButton = null;
    
    #if BUILD_MODE
    Button settingButton = null;
    #endif
    
    Slider slider = null;
    YSliderChange sliderChange = null;
    
    public float loadFakeTime = 0.5f;
    public float loadFinalTime = 0.1f;
    
    static readonly string pathPlace = "Prefabs/YPlace/HLoadGamePlace";
    private GameObject loadGamePlace;
    public YLoadPanel() : base(new UIType(path)){}
    
    public override void OnEnter()
    {
        loadGamePlace = GameObject.Instantiate(Resources.Load<GameObject>(pathPlace));
        
        YTriggerEvents.OnLoadResourceStateChanged += Loadend;
        enterGameButton = uiTool.GetOrAddComponentInChilden<Button>("EnterGameButton");
        enterGameButton.gameObject.SetActive(false);
        //2s后出现按钮EnterGameButton
        //Invoke("ShowEnterGameButton",2f);
        settingButton = uiTool.GetOrAddComponentInChilden<Button>("SettingButton");
        settingButton.onClick.AddListener(() =>
        {
            Push(new SettingPanel());
        });
        TMP_Text descriptText = uiTool.GetOrAddComponentInChilden<TMP_Text>("DescriptionText");
        #if BUILD_MODE
        descriptText.text = "游戏版本：0.0.6\n仅为学习项目使用";
        //todo:设置按钮的功能，后面做一下，绑定一个监听事件
        #else
            descriptText.text = "开发版本：0.0.7";
        #endif
        //加载进度条
        // uiTool.GetOrAddComponentInChilden<Slider>("LoadSlider")
        //     .GetComponent<YSliderChange>().SetSliderOn(0,0.9f,0.2f);
        slider = uiTool.GetOrAddComponentInChilden<Slider>("LoadSlider");
        sliderChange = slider.GetComponent<YSliderChange>();
        sliderChange.SetSliderOn(0,0.9f,loadFakeTime,slider,this);  
    }
    public override void OnExit()
    {
        base.OnExit();
        YTriggerEvents.OnLoadResourceStateChanged -= Loadend;
    }
    public void ShowEnterGameButton()
    {
        if (enterGameButton == null)
        {
            Debug.LogError("enterGameButton为空");
            return;
        }
        enterGameButton.gameObject.SetActive(true);
        enterGameButton.onClick.AddListener(()=>
        {
            HMessageShowMgr.Instance.ShowMessageWithActions("CONFIRM_MIHOYO_GAME", ConfirmMihoyo, CancelMihoyo, CancelMihoyo);
        });
    }

    private void ConfirmMihoyo()
    {
        yPlanningTable.Instance.isMihoyo = true;
        StartRealGameWithOption();
    }
    
    private void CancelMihoyo()
    {
        yPlanningTable.Instance.isMihoyo = false;
        StartRealGameWithOption();
    }

    private void StartRealGameWithOption()
    {
        // Pop();
        // panelManager.Push(new StartPanel());
        // Debug.Log("点击了开始游戏按钮");
        // GameObject.Destroy(loadGamePlace, 1f);  //以上四行是原来的流程
        
        Pop();
        YLevelManager.JustSetCurrentLevelIndex(0);
        yPlanningTable.Instance.EnterNewLevel(0);
        //Push(new YLevelPanel(0));//游玩模式 //这是现在的流程
        YPlayModeController.Instance.StartGameAndSet(10,""); //暂时先写死，是我们的主角在场景里
        GameObject.Destroy(loadGamePlace, 1f);  
    }
    
    void Loadend(object sender, YTriggerEventArgs e)
    {
        if (sliderChange == null)
        {
            slider = uiTool.GetOrAddComponentInChilden<Slider>("LoadSlider");
            sliderChange = slider.GetComponent<YSliderChange>();
        }
        
        sliderChange.StopCoroutine("CountDown");
        
        sliderChange.SetSliderOn(0.9f,1f,loadFinalTime,slider,this);
        //等待0.1s后
        sliderChange.Invoke("endLoad",loadFinalTime);
    }
    public void HideSlider()
    {
        slider.gameObject.SetActive(false);
        //播放对应的Timeline，然后展示进入游戏的界面
        loadGamePlace.GetComponent<HTimelineTriggerSomething>().LoadScenePlayDirector(this, 5f);
    }
    
}