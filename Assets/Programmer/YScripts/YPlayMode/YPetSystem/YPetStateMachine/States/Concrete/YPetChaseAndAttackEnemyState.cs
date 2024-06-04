using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 边追边打
/// </summary>
public class YPetChaseAndAttackEnemyState : YPetBaseState
{
    private YPetBase mPatrolAI;
    Transform chaseTarget;
    public YPetChaseAndAttackEnemyState (YPetBase yPetBase) : base(yPetBase.gameObject)
    {
        mPatrolAI = yPetBase;
    }

    public override Type Tick()
    {
        if(mPatrolAI.mNavMeshAgent.enabled == false)
            return null;
        
        //如果距离角色远 或者 没有敌人，都会就切换到攻击状态
        // if (!mPatrolAI.IsCloseEnoughToCharacter() || !mPatrolAI.CheckEnemyCount())
        if (mPatrolAI.ChaseTarget == null || !mPatrolAI.CheckEnemyCount())
        {
            ExitState();
            return typeof(YPetFollowState);
        }
        //还是要一直设置目的地的吧？navmeshagent的destination是不是会自动更新？
        mPatrolAI.mNavMeshAgent.SetDestination(mPatrolAI.ChaseTarget.position);
        return null;
    }
    
    //enter
    public override void OnStateEnter()
    {
        chaseTarget = mPatrolAI.CheckAndGetEnemy().transform;
        //打开navmeshagent
        mPatrolAI.mNavMeshAgent.enabled = true;
        mPatrolAI.MuzzleShoot();
    }
    private void ExitState()
    {
        mPatrolAI.MuzzleStopShoot();
    }

    
    
}
