
using UnityEngine;
using System;

public class YTriggerEventArgs : EventArgs
{
    public bool activated;
}

public class YTriggerEvents : MonoBehaviour
{
    public static event EventHandler<YTriggerEventArgs> OnTriggerStateChanged;
    //这句话是定义一个事件，这个事件是一个委托，这个委托是一个EventHandler类型的委托，这个委托的参数是一个object类型的sender和一个YTriggerEventArgs类型的e

    public static void RaiseOnTriggerStateChanged(bool activated)
    {
        OnTriggerStateChanged?.Invoke(null, new YTriggerEventArgs { activated = activated });
    }
    
    //定义一个事件 当可以呼出木偶面板的时候 可以使用快捷键进行呼出 
    public static event EventHandler<YTriggerEventArgs> OnPuppetShortCutStateChanged;
    public static void RaiseOnPuppetShortCutStateChanged(bool activated)
    {
        OnPuppetShortCutStateChanged?.Invoke(null, new YTriggerEventArgs { activated = activated });
    }
    
    //鼠标锁定和解锁
    public static event EventHandler<YTriggerEventArgs> OnMouseLockStateChanged;
    public static void RaiseOnMouseLockStateChanged(bool activated)
    {
        OnMouseLockStateChanged?.Invoke(null, new YTriggerEventArgs { activated = activated });
    }
    
    //启用快捷键分屏事件
    public static event EventHandler<YTriggerEventArgs> OnShortcutKeySplitScreenStateChanged;
    public static void RaiseOnShortcutKeySplitScreenStateChanged(bool activated)
    {
        OnShortcutKeySplitScreenStateChanged?.Invoke(null, new YTriggerEventArgs { activated = activated });
    }
    
}