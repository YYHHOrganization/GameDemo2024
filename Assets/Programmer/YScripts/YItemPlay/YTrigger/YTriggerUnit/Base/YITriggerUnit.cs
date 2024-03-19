using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface YITriggerUnit
{
    public event Action<bool> OnTriggerStateChanged; // 定义状态改变事件
    public event Action<GameObject> OnEnterFieldStateChanged; // 定义进入触发区域事件
}
