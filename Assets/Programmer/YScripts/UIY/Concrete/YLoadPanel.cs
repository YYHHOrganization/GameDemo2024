using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YLoadPanel : BasePanel
{
    static readonly string path = "Prefabs/UI/Panel/YLoadPanel";
    Button enterGameButton = null;
    
    Slider slider = null;
    YSliderChange sliderChange = null;
    
    public float loadFakeTime = 0.5f;
    public float loadFinalTime = 0.1f;
    public YLoadPanel() : base(new UIType(path)){}
    
    public override void OnEnter()
    {
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
    void ShowEnterGameButton()
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
        ShowEnterGameButton();
    }
    
}