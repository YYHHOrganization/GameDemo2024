
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class YTriggerEventArgs : EventArgs
{
    public bool activated;
}
//还包含物体GameObject
public class YTriggerGameObjectEventArgs : EventArgs
{
    public bool activated;
    public GameObject gameObject;
    public Transform showUIPlace;
}

//有 activated 用来看是否开启快捷键，以及需要有一个对应，
// public class YShortCutKeyEventArgs : EventArgs
// {
//     public bool activated;
//     public InputActionMap inputActionMap;
// }

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
    
    //启动出现提示面板，并可以使用快捷键进行交互
    public static event EventHandler<YTriggerGameObjectEventArgs> OnShortcutKeyInteractionStateChanged;
    public static void RaiseOnShortcutKeyInteractionStateChanged(bool activated,GameObject go,Transform showUIPlace)
    {
        OnShortcutKeyInteractionStateChanged?.Invoke(null,
            new YTriggerGameObjectEventArgs { activated = activated, gameObject = go,showUIPlace = showUIPlace });
    }
    
    
    //启用快捷键或者不启用
    public static event EventHandler<YTriggerEventArgs> OnShortcutKeyEsc;
    // public static void RaiseOnShortcutKeyEnableOrDisable(bool activated,InputActionMap inputActionMap)
    public static void RaiseOnShortcutKeyEsc(bool activated)
    
    {
       OnShortcutKeyEsc?.Invoke(null, new YTriggerEventArgs { activated = activated });
    }
    
    //是否加载完资源
    public static event EventHandler<YTriggerEventArgs> OnLoadResourceStateChanged;
    public static void RaiseOnLoadResourceStateChanged(bool activated)
    {
        OnLoadResourceStateChanged?.Invoke(null, new YTriggerEventArgs { activated = activated });
    }
}