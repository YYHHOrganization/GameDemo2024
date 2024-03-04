using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 所有panel的基类 包含UI面板的基本信息
/// </summary>
public abstract class BasePanel
{
    /// <summary>UI信息
    public UIType UIType { get; private set; }
    public UITool uiTool { get; private set; }
    
    public PanelManager panelManager{ get; private set; }
    public UIManager uiManager { get; private set; }
    public BasePanel(UIType uiType)
    {
        UIType = uiType;
    }
    /// <summary>
    /// 初始化UITool
    /// </summary>
    /// <param name="tool"></param>
    public void Initialize(UITool tool)
    {
        uiTool = tool;
    }
    /// <summary>
    /// 初始化面板管理器
    /// </summary>
    /// <param name="manager"></param>
    public void Initialize(PanelManager manager)
    {
        panelManager = manager;
    }
    public void Initialize(UIManager manager)
    {
        uiManager = manager;
    }
    
    /// <summary>UI进入时执行的操作 只会执行一次
    public virtual void OnEnter() { }

    /// <summary>UI暂停时执行的操作
    public virtual void OnPause()
    {
        //先判断是否为空
        if (uiTool != null)
        {
            //获取CanvasGroup组件 判断是否为空：
            if (uiTool.Get<CanvasGroup>() != null)
            {
                //设置blocksRaycasts为false
                uiTool.Get<CanvasGroup>().blocksRaycasts = false;
            }
            else
            {
                //如果为空，打印错误信息
                Debug.LogError("CanvasGroup is not found");
            }
        }
            
        //uiTool.Get<CanvasGroup>().blocksRaycasts = false;
    }

    /// <summary>UI恢复时执行的操作
    public virtual void OnResume()
    {
        uiTool.Get<CanvasGroup>().blocksRaycasts = true;
    }

    /// <summary>UI退出时执行的操作
    public virtual void OnExit()
    {
        uiManager.DestroyUI(UIType);
    }
    
    /// <summary>
    /// 显示一个面板
    /// </summary>
    /// <param name="panel"></param>
    public void Push(BasePanel panel)=>panelManager?.Push(panel);
    /// <summary>
    /// 关闭一个面板
    /// </summary>
    public void Pop()=>panelManager?.Pop();
}
