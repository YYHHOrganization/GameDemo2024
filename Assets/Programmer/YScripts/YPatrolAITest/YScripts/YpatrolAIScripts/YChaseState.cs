using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YChaseState : YBaseState
{
    private YPatrolAI patrolAI;
    public YChaseState(YPatrolAI yPatrolAI) :base(yPatrolAI.gameObject)
    {
        patrolAI = yPatrolAI;
    }
    public override Type Tick()
    {
        
        if (patrolAI.mTarget == null) 
        {
            patrolAI.animator.SetInteger("AnimState", 0);
            return typeof(YWanderState);
        }

        //transform.LookAt(patrolAI.mTarget);
        //transform.Translate(Vector3.forward*Time.deltaTime*YGameSetting.PatrolAISpeed);
        patrolAI.mNavMeshAgent.destination = patrolAI.mTarget.position;


        if (Vector3.Distance(transform.position,patrolAI.mTarget.transform.position)<=YGameSetting.AttackRange)
        {
            patrolAI.animator.SetInteger("AnimState",2);
            return typeof(YAttackState);
        }

        if (Vector3.Distance(transform.position, patrolAI.mTarget.transform.position) > YGameSetting.ChaseRange)
        {
            patrolAI.animator.SetInteger("AnimState", 0);
            if (patrolAI.SpotLightWander) patrolAI.SpotLightWander.SetActive(true);
            if (patrolAI.SpotLightChase) patrolAI.SpotLightChase.SetActive(false);
            // patrolAI.mNavMeshAgent.destination= null;
            //不再追踪
            patrolAI.mNavMeshAgent.ResetPath();
            return typeof(YWanderState);
        }
        if (patrolAI.isDead == true)
        {
            //不再追踪
            patrolAI.mNavMeshAgent.ResetPath();
            return typeof(YDieState);
        }
        return null;
    }
}
