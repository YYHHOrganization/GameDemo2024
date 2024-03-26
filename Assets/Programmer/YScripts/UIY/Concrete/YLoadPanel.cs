using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class YLoadPanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/YLoadPanel";
    Button enterGameButton = null;
    
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
        
        //加载进度条
        // uiTool.GetOrAddComponentInChilden<Slider>("LoadSlider")
        //     .GetComponent<YSliderChange>().SetSliderOn(0,0.9f,0.2f);
        slider = uiTool.GetOrAddComponentInChilden<Slider>("LoadSlider");
        sliderChange = slider.GetComponent<YSliderChange>();
        sliderChange.SetSliderOn(0,0.9f,loadFakeTime,slider,this);  
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
            Pop();
            panelManager.Push(new StartPanel());
            Debug.Log("点击了开始游戏按钮");
            GameObject.Destroy(loadGamePlace, 10f);
        });
    }
    
    void Loadend(object sender, YTriggerEventArgs e)
    {
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