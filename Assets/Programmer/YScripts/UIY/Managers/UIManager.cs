using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    /// <summary>
    /// 存储所有UI信息的字典 每一个UI信息都会对应一个GameObject
    /// TODO:这UIDic里的UIType可能只能只能有一个，可以优化
    /// </summary>
    private Dictionary<UIType, GameObject> UIDic;
    public UIManager()
    {
        UIDic = new Dictionary<UIType, GameObject>();
    }
    
    /// <summary>
    /// 获取一个UI对象
    /// </summary>
    /// <param name="uiType"></param>
    /// <returns></returns>
    public GameObject GetSingleUI(UIType uiType)
    {
        GameObject parent = GameObject.Find("Canvas");
        if (parent == null)
        {
            Debug.LogError("Canvas is not found");
            return null;
        }
        if (UIDic.ContainsKey(uiType))
        {
            return UIDic[uiType];
        }
        else
        {
            GameObject ui = GameObject.Instantiate(Resources.Load<GameObject>(uiType.Path), parent.transform);
            //设置UI的名字
            ui.name = uiType.Name;
            UIDic.Add(uiType, ui);
            return ui;
        }
    }
    
    /// <summary>
    /// 销毁一个UI对象
    /// </summary>
    /// <param name="uiType"></param>
    public void DestroyUI(UIType uiType)
    {
        if (UIDic.ContainsKey(uiType))
        {
            //不确定啊 教程这里也错了
            GameObject.Destroy(UIDic[uiType]);
            UIDic.Remove(uiType);
        }
    }

}
