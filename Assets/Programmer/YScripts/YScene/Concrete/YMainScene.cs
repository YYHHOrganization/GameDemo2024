using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Main场景
/// </summary>
public class YMainScene : YSceneState
{
    //名称
    readonly string m_SceneName = "YMainScene";
    PanelManager panelManager;
    public override void OnEnter()
    {
        //Debug.Log("进入" + m_SceneName);
        panelManager = new PanelManager();//这里是不是可优化？
        if (SceneManager.GetActiveScene().name != m_SceneName)
        {
            SceneManager.LoadScene(m_SceneName);
            //加载完之后执行的操作
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            //如果已经加载过了，直接加载UI
            panelManager.Push(new YMainPanel());
        }
        
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
        panelManager.Push(new YMainPanel());
    }
}
