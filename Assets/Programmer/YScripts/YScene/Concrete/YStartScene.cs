using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// 开始场景
/// </summary>
public class YStartScene : YSceneState
{
    //名称
    readonly string m_SceneName = "YStartScene";
    PanelManager panelManager;
    public override void OnEnter()
    {
        
        //Debug.Log("进入" + m_SceneName);
        panelManager = new PanelManager();//这里是不是可优化？
        // panelManager.Push(new StartPanel());
        panelManager.Push(new YLoadPanel());
        YGameRoot.Instance.SetAction(panelManager.Push);
        YGameRoot.Instance.SetAction(panelManager.Pop);
        
        // if (SceneManager.GetActiveScene().name != m_SceneName)
        // {
        //     SceneManager.LoadScene(m_SceneName);
        //     //加载完之后执行的操作
        //     SceneManager.sceneLoaded += OnSceneLoaded;
        // }
        // else
        // {
        //     //如果已经加载过了，直接加载UI
        //     panelManager.Push(new StartPanel());
        // }
    }
    public override void OnExit()
    {
        //Debug.Log("离开" + m_SceneName);
        SceneManager.sceneLoaded -= OnSceneLoaded;
        panelManager.popAll();
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log(m_SceneName + "加载完毕");
        
        //加载完之后执行的操作：加载UI
        // panelManager.Push(new StartPanel());
        panelManager.Push(new YLoadPanel());
        YGameRoot.Instance.SetAction(panelManager.Push);//这句话等价于用委托链表：YGameRoot.Instance.Push += panelManager.Push！！
        YGameRoot.Instance.SetAction(panelManager.Pop);
    }
}
