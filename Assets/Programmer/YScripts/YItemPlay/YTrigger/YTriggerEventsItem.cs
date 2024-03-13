
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
}