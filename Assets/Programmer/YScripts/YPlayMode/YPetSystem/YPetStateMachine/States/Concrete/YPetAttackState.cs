using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 跟随状态 跟随当前角色
/// </summary>
public class YPetAttackState : YPetBaseState
{
    private YPetBase mPatrolAI;

    public YPetAttackState(YPetBase yPetBase) : base(yPetBase.gameObject)
    {
        mPatrolAI = yPetBase;
    }

    public override Type Tick()
    {
        return null;//test
        //如果距离角色远 或者 没有敌人，都会就切换到攻击状态
        if (!mPatrolAI.IsCloseEnoughToCharacter() || !mPatrolAI.CheckEnemyCount())
        {
            ExitState();
            return typeof(YPetFollowState);
        }
        
        return null;
    }

    private void ExitState()
    {
        mPatrolAI.MuzzleStopShoot();
    }

    private void SetNavmeshDestination()
    {
        if (mPatrolAI.curCharacterTrans)
        {
            if (mPatrolAI)
            {
                mPatrolAI.mNavMeshAgent.destination = mPatrolAI.curCharacterTrans.position;
                //trick:感觉这里给敌人设置不同的速度等参数会更好，不然召唤出来之后他们的逻辑都是完全一样的
                // mPatrolAI.mNavMeshAgent.speed = Random.Range(1, mPatrolAI.enemy._RogueEnemyChaseMaxSpeed());
                // mPatrolAI.mNavMeshAgent.acceleration = Random.Range(2, mPatrolAI.chaseMaxAcceleration);
            }
        }
    }

    //enter
    public override void OnStateEnter()
    {
        //打开navmeshagent
        //mPatrolAI.mNavMeshAgent.enabled = true;
        mPatrolAI.MuzzleShoot();
        //
        Debug.Log("Attack");
    }
}