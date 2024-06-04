using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityVector2;
using UnityEngine;

/// <summary>
/// 只追不打
/// </summary>
public class YPetChaseState  : YPetBaseState
{
    private YPetBase mPatrolAI;
    Transform chaseTarget;
    public  YPetChaseState  (YPetBase yPetBase) : base(yPetBase.gameObject)
    {
        mPatrolAI = yPetBase;
    }

    public override Type Tick()
    {
        if(mPatrolAI.mNavMeshAgent.enabled == false)
            return null;

        if (!mPatrolAI.CheckEnemyCount())
        {
            ExitState();
            return typeof(YPetFollowState);
        }
        //如果距离角色远 或者 没有敌人，都会就切换到攻击状态
        // if (!mPatrolAI.IsCloseEnoughToCharacter() || !mPatrolAI.CheckEnemyCount())
        if (mPatrolAI.ChaseTarget == null )
        {
            GameObject enemy = mPatrolAI.CheckAndGetEnemy();
            if(enemy!=null)
            {
                mPatrolAI.ChaseTarget = enemy.transform;
            }
            else
            {
                ExitState();
                return typeof(YPetFollowState);
            }
            
        }
        //还是要一直设置目的地的吧？navmeshagent的destination是不是会自动更新？
        mPatrolAI.mNavMeshAgent.SetDestination(mPatrolAI.ChaseTarget.position);
        
        //这里应该有一个是说 如果追到了敌人，即敌人在视野内，那么就切换到攻击状态
        //敌人在视野内,与敌人的距离小于攻击距离
        
        if (Vector3.Distance(mPatrolAI.ChaseTarget.position, mPatrolAI.transform.position) < mPatrolAI.AttackDistance)
        {
            ExitState();
            return typeof(YPetAttackState);
        }
        
        return null;
    }
    
    //enter
    public override void OnStateEnter()
    {
        mPatrolAI.EnterChaseState();
        
        //打开navmeshagent
        mPatrolAI.mNavMeshAgent.enabled = true;
        //mPatrolAI.MuzzleShoot();
    }
    private void ExitState()
    {
        //mPatrolAI.MuzzleStopShoot();
    }

    
    
}
