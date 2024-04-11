using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HRogueEnemyCommonStateMachine : MonoBehaviour
{
    private Dictionary<Type, HRogueEnemyBaseState> mDicAvailStates;

    public HRogueEnemyBaseState CurrentState { get; private set; }
    //事件***
    public event Action<HRogueEnemyBaseState> OnStateChanged;

    public void SetStates(Dictionary<Type,HRogueEnemyBaseState> dicStates)
    {
        mDicAvailStates = dicStates;
    }
    
    void Update()
    {
        //如果此时没有状态 将第一个状态赋值给他
        if(CurrentState==null)
        {
            CurrentState = mDicAvailStates.Values.First();
            CurrentState.OnStateEnter();
        }
        //basestate脚本中有方法tick  如果并不切换状态 tick就返回null 否则就是说明要切换状态
        //如果下一状态与以此时的状态一样 不管tick与否 都不需要换状态，否则要换
        var nextState = CurrentState?.Tick();
           
        //切换状态
        if(nextState!=null&&nextState!=CurrentState.GetType())
        {
            switchState(nextState);
        }
    }

    private void switchState(Type nextState)
    {
        if (nextState != null)
        {
            CurrentState = mDicAvailStates[nextState];
            // OnStateEnter() { }
            CurrentState.OnStateEnter();
            OnStateChanged?.Invoke(CurrentState);
        }
    }
}
