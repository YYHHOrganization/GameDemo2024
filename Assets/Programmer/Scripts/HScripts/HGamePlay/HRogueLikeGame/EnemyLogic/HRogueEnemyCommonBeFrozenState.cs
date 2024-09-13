using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HRogueEnemyCommonBeFrozenState : HRogueEnemyBaseState
{
    private HRogueEnemyPatrolAI mPatrolAI;
    private HRogueEnemyBaseState m_PreviousState;
    public HRogueEnemyCommonBeFrozenState(HRogueEnemyPatrolAI patrolAI) : base(patrolAI.gameObject)
    {
        mPatrolAI = patrolAI;
    }

    public override Type Tick()
    {
        if (mPatrolAI.isDead)
            return null;
        if (!mPatrolAI.EnemyIsFrozen)
        {
            //停止冰冻
            mPatrolAI.ExitFronzenState();
            
            return typeof(HRogueEnemyCommonChaseState);
        }
        return null;
    }

    public override void OnStateEnter()
    {
        mPatrolAI.StopAllCoroutines();
        if (mPatrolAI.mNavMeshAgent != null && mPatrolAI.mNavMeshAgent.enabled)
        {
            mPatrolAI.mNavMeshAgent.isStopped = true;
        }
        mPatrolAI.StartCoroutine(mPatrolAI.FrozenEnemyItself());
    }
    //exit
    
}
