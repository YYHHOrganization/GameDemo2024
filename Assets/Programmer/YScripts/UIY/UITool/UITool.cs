using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// UI管理工具，包括获取某个子对象组件的操作
/// </summary>
public class UITool
{
    GameObject activePanel;
    public UITool(GameObject ActivePanel)
    {
        activePanel = ActivePanel;
    }
    /// <summary>
    /// 获得或给当前的面板添加一个组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <returns></returns>
    public T Get<T>() where T : Component
    {
        if (activePanel==null)
        {
            Debug.LogError("当前面板为空");
            return null;
        }
        
        if (activePanel.GetComponent<T>() == null)
        {
            activePanel.AddComponent<T>();
        }
        return activePanel.GetComponent<T>();
        
        // if (activePanel != null)
        // {
        //     if (activePanel.GetComponent<T>()==null)
        //     {
        //         activePanel.AddComponent<T>();
        //     }
        // }
        // return activePanel.GetComponent<T>();

    }
    /// <summary>
    /// 根据名称寻找当前面板的一个子对象
    /// </summary>
    public GameObject FindChildGameObject(string name)
    {
        Transform[] trans = activePanel.GetComponentsInChildren<Transform>();
        foreach (Transform item in trans)
        {
            if(item.name==name)
            {
                return item.gameObject;
            }
        }
        Debug.LogError("没有找到名字为"+name+"的子对象");
        return null;
    }
    
    /// <summary>
    /// 根据名称获取一个子对象的组件，如果没有则添加一个
    /// </summary>
    /// <param name="name"> 子对象名称</param>
    /// <typeparam name="T"> 组件类型</typeparam>
    /// <returns></returns>
    public T GetOrAddComponentInChilden<T>(string name) where T : Component
    {
        GameObject go = FindChildGameObject(name);
        if (go != null)
        {
            if (go.GetComponent<T>() == null)
            {
                go.AddComponent<T>();
            }
            return go.GetComponent<T>();
        }
        return null;
    }
}
