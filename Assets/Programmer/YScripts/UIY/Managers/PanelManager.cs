using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 面板管理器 用栈存储所有的面板
/// </summary>
public class PanelManager 
{
    Stack<BasePanel> panelStack;
    private UIManager uiManager;
    private BasePanel panel;
    public PanelManager()
    {
        panelStack = new Stack<BasePanel>();
        uiManager = new UIManager();//UIManager是一个单例会更好吧、、？？、
    }
    
    /// <summary>
    /// UI的入栈操作 此操作会触发当前栈顶的UI的暂停操作 并且会触发新的UI的进入操作 会显示一个面板
    /// </summary>
    /// <param name="nextPanel"></param>
    public void Push(BasePanel nextPanel)
    {
        Debug.Log("panelStack.Count : " + panelStack.Count);
        if (panelStack.Count > 0)
        {
            panel = panelStack.Peek();
            panel.OnPause();
        }
        panelStack.Push(nextPanel);

        GameObject panelGO = uiManager.GetSingleUI(nextPanel.UIType);
        nextPanel.Initialize(new UITool(panelGO));
        nextPanel.Initialize(this);
        nextPanel.Initialize(uiManager);
        nextPanel.OnEnter();
    }
    /// <summary>
    /// 执行栈顶的面板UI的退出操作 并且会触发新的栈顶的UI的恢复操作
    /// </summary>
    public void Pop()
    {
        if (panelStack.Count > 0)
        {
            panel = panelStack.Pop();
            panel.OnExit();
        }
        
        if (panelStack.Count > 0)
        {
            panel = panelStack.Peek();
            panel.OnResume();
        }
    }
    /// <summary>
    /// 执行所有面板的OnExit操作
    /// </summary>
    public void popAll()
    {
        while(panelStack.Count > 0)
        {
            //panelStack.Pop().OnExit();
            panel = panelStack.Pop();
            panel.OnExit();
        }
    }
}
