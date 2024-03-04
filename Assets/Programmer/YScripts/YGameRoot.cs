using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class YGameRoot : MonoBehaviour
{
    //单例模式
    public static YGameRoot Instance{get;private set;}
    public YSceneSystem SceneSystem{get;private set;}
    
    /// <summary>
    /// 为了在框架外部能够监听到面板的push操作 显示一个面板
    /// UnityAction是一个委托类型 用于监听事件，当事件发生时，所有注册的方法都会被调用，此处用于监听面板的push操作
    /// </summary>
    //public UnityAction<BasePanel> OnPanelPush;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        SceneSystem = new YSceneSystem();
        //DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        //进入开始场景
        SceneSystem.SetScene(new YStartScene());
    }
    
    /// <summary>
    /// 设置监听事件push
    /// </summary> 
    // public void SetAction(UnityAction<BasePanel> pushAction)
    // {
    //     OnPanelPush = pushAction;
    // }
}
