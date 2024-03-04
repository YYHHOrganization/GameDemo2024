using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 场景的状态管理系统
/// </summary>
public class YSceneSystem 
{
    YSceneState m_State;
    
    public void SetScene(YSceneState state)
    {
        //退出当前场景
        // if (m_State != null)
        // {
        //     m_State.OnExit();
        // }
        m_State?.OnExit();//等价于上面的if判断
        //进入新场景
        m_State = state;
        // if (m_State != null)
        //     m_State.OnEnter();
        m_State?.OnEnter();
    }
    
}
