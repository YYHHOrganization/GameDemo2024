
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
//给抽奖机，传的物体
public class YTriggerGivingItemEventArgs : EventArgs
{
    public string itemId;
    public int itemCount;
}
public class YTriggerCountEventArgs : EventArgs
{
    public bool activated;
    public int count;
}

///肉鸽逻辑，进入一个信房间，传入房间类型和true or false
public class YTriggerEnterRoomTypeEventArgs : EventArgs
{
    public bool activated;
    public RoomType roomType;
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
    
    //是否启用快捷键-进入锁住视角
    public static event EventHandler<YTriggerEventArgs> OnShortcutKeyLockView;
    public static void RaiseOnShortcutKeyLockViewStateChanged(bool activated)
    {
        OnShortcutKeyLockView?.Invoke(null, new YTriggerEventArgs { activated = activated });
    }
    
    //是否进入新的一关/重新开始 很多enter new level都没有用这个的逻辑，因为之前就写好了(这个是总关卡 不是肉鸽那个层)
    public static event EventHandler<YTriggerEventArgs> OnEnterNewLevel;

    public static void RaiseOnEnterNewLevel(bool activated)
    {
        OnEnterNewLevel?.Invoke(null, new YTriggerEventArgs { activated = activated });
    }
    
    //是否进入新的肉鸽那个层!和上面那个不同！！！！
    public static event EventHandler<YTriggerEventArgs> OnEnterRogueNewLevel;

    public static void RaiseOnEnterRogueNewLevel(bool activated)
    {
        OnEnterRogueNewLevel?.Invoke(null, new YTriggerEventArgs { activated = activated });
    }


    //给抽奖机物品点击确认
    public static event EventHandler<YTriggerGivingItemEventArgs> OnGiveOutItemInBagForSlotMachine;
    public static void RaiseOnGiveOutItemInBagForSlotMachine(string id, int count)
    {
        OnGiveOutItemInBagForSlotMachine?.Invoke(null, new YTriggerGivingItemEventArgs{itemId = id, itemCount = count});
    }
    
    //是否开启鼠标左键射击的监听
    public static event EventHandler<YTriggerEventArgs> OnMouseLeftShoot;
    public static void RaiseOnMouseLeftShoot(bool activated)
    {
        OnMouseLeftShoot?.Invoke(null, new YTriggerEventArgs { activated = activated });
    }
    
    //肉鸽逻辑，完成某个房间，例如完成冒险房的答题之后，可以开启相应的宝箱
    public static event EventHandler<YTriggerCountEventArgs> OnCompleteRoom;
    public static void RaiseOnCompleteRoom(bool activated  ,int count)
    {
        OnCompleteRoom?.Invoke(null, new YTriggerCountEventArgs { activated = activated, count = count });
    }
    
    //肉鸽逻辑，进入一个信房间，传入房间类型和true or false
    public static event EventHandler<YTriggerEnterRoomTypeEventArgs> OnEnterRoomType;
    public static void RaiseOnEnterRoomType(bool activated, RoomType roomType)
    {
        OnEnterRoomType?.Invoke(null, new YTriggerEnterRoomTypeEventArgs { activated = activated, roomType = roomType });
    }
    
    //肉鸽音游逻辑（别的希望也能复用），是否中断Combo
    public static event EventHandler<YTriggerEventArgs> OnInterruptCombo;
    public static void RaiseOnInterruptCombo(bool activated)
    {
        OnInterruptCombo?.Invoke(null, new YTriggerEventArgs { activated = activated });
    }
    
    //加载完load界面 真正出现在地牢里面
    public static event EventHandler<YTriggerEventArgs> OnLoadEndAndBeginPlay;
    public static void RaiseOnLoadEndAndBeginPlay(bool activated)
    {
        OnLoadEndAndBeginPlay?.Invoke(null, new YTriggerEventArgs { activated = activated });
    }
}