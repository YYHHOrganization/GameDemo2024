using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public class YGameRoot : MonoBehaviour
{
    //单例模式
    public static YGameRoot Instance{get;private set;}
    public YSceneSystem SceneSystem{get;private set;}

    /// <summary>
    /// 为了在框架外部能够监听到面板的push操作 显示一个面板
    /// UnityAction是一个委托类型 用于监听事件，当事件发生时，所有注册的方法都会被调用，此处用于监听面板的push操作
    /// 定义了一个公共属性 Push，用于存储一个可以处理 BasePanel 参数的委托，
    /// </summary>
    public UnityAction<BasePanel> Push { get; private set; }
    public UnityAction Pop { get; private set; }

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
    /// 委托-函数指针
    /// <BasePanel>是委托的参数类型 
    /// </summary> 
    public void SetAction(UnityAction<BasePanel> pushAction)
    {
        Push = pushAction;
    }
    public void SetAction(UnityAction popAction)
    {
        Pop = popAction;
    }

    private void Update()
    {
        // //如果点击F //呼出木偶菜单
        // if (Input.GetKeyDown(KeyCode.F))
        // {
        //     //进入开始场景
        //     Pop();
        //     Push(new YChooseScreenplayInPlayModePanel());
        //     YPlayModeController.Instance.SetCameraLayout(2);
        // }
    }
    
    public void QuitGame()
        {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
            Application.Quit();
    #endif
        }
    public void PlayAgain()
    {
        // SceneSystem.SetScene(new YStartScene());
        

        SceneManager.LoadScene ("Level1DemoScene");
        
    }
    
    
}
