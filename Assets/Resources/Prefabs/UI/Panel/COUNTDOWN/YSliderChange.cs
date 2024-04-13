using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YSliderChange : MonoBehaviour
{
    //给定时间和start和end 让slider的value值在start和end之间变化
    public float time = 1f;
    public float start = 0f;
    public float end = 1f;
    private float timer = 0f;
    public YLoadPanel yLoadPanel;
    public YRogueLoadPanel yRogueLoadPanel;
    
    //协程
    IEnumerator CountDown(float start, float end, float duration,Slider slider)
    {
        float time = duration;
        float timer = 0f;
        while (timer <= time)
        {
            timer += Time.deltaTime;
            float t = timer / time;
            float value = Mathf.Lerp(start, end, t);
            slider.value = value;
            yield return null;
        }
    }
    
    
    public void SetSliderOn(float start, float end, float duration,Slider slider,YLoadPanel yLoadPanel)    
    {
        this.start = start;
        this.end = end;
        this.time = duration;
        timer = 0f;
        this.yLoadPanel = yLoadPanel;
        StartCoroutine(CountDown(start, end, duration,slider));
        
    }
    public void SetSliderOn(float start, float end, float duration,Slider slider,YRogueLoadPanel yLoadPanel) 
    {
        this.start = start;
        this.end = end;
        this.time = duration;
        timer = 0f;
        this.yRogueLoadPanel = yLoadPanel;
        StartCoroutine(CountDown(start, end, duration,slider));
        
    }
    public void endLoad()
    {
        yLoadPanel.HideSlider();
    }
   
}
