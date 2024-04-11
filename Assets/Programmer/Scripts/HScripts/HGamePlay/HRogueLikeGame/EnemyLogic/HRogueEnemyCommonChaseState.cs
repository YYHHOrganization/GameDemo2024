using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HRogueEnemyCommonChaseState : HRogueEnemyBaseState
{
    private HRogueEnemyPatrolAI mPatrolAI;
    public HRogueEnemyCommonChaseState(HRogueEnemyPatrolAI patrolAI) : base(patrolAI.gameObject)
    {
        mPatrolAI = patrolAI;
    }
    
    public override Type Tick()
    {
        if (mPatrolAI.mNavMeshAgent.enabled)
        {
            mPatrolAI.mNavMeshAgent.destination = mPatrolAI.mTarget.position;
        }

        if (mPatrolAI.isDead)
            return null;
        
        // /transform.LookAt(patrolAI.mTarget);
        // //transform.Translate(Vector3.forward*Time.deltaTime*YGameSetting.PatrolAISpeed);
        //
        // if (mPatrolAI.CheckIfTargetIsNear(mPatrolAI.attackRange))
        // {
        //     mPatrolAI.animator.SetBool(mPatrolAI.IsAttackingHash, true);
        //     //return typeof(YAttackState);
        // }
        //
        // if (mPatrolAI.isDead == true)
        // {
        //     //不再追踪
        //     mPatrolAI.mNavMeshAgent.enabled = false;
        //     // patrolAI.mNavMeshAgent.ResetPath();
        //     return typeof(HRogueEnemyCommonDieState);
        // }
        return null;
    }

    private void SetNavmeshDestination()
    {
        if (mPatrolAI.mTarget)
        {
            mPatrolAI.mNavMeshAgent.destination = mPatrolAI.mTarget.position;
        }
    }
    
    //enter
    public override void OnStateEnter()
    {
        mPatrolAI.mNavMeshAgent.enabled = true;
        
        //根据chase类型决定接下来的逻辑
        if (mPatrolAI.chaseType == RogueEnemyChaseType.JustChase)
        {
            SetNavmeshDestination();
        }
        else if (mPatrolAI.chaseType == RogueEnemyChaseType.ChaseAndShootPlayer)
        {
            SetNavmeshDestination();
            mPatrolAI.ShootBulletForward(true,true);
        }
    }
}
