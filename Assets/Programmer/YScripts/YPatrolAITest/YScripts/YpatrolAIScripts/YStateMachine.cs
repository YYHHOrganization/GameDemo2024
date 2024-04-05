using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class YStateMachine : MonoBehaviour
{
    private Dictionary<Type, YBaseState> mDicAvailStates;

    public YBaseState CurrentState { get; private set; }
    //�¼�***
    public event Action<YBaseState> OnStateChanged;

    public void SetStates(Dictionary<Type,YBaseState> dicStates)
    {
        mDicAvailStates = dicStates;
    }

    // Update is called once per frame
    void Update()
    {
        //�����ʱû��״̬ ����һ��״̬��ֵ����
        if(CurrentState==null)
        {
            CurrentState = mDicAvailStates.Values.First();
        }
        //Ybasestate���з���tick  ��������л�״̬ tick�ͷ���null �������˵��Ҫ�л�״̬
        //�����һ״̬���Դ�ʱ��״̬һ�� ����tick��� ������Ҫ��״̬������Ҫ��
        var nextState = CurrentState?.Tick();
           
        //�л�״̬
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
