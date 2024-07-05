using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;

public class HRogueEnemyCommonChaseState : HRogueEnemyBaseState
{
    private HRogueEnemyPatrolAI mPatrolAI;
    public HRogueEnemyCommonChaseState(HRogueEnemyPatrolAI patrolAI) : base(patrolAI.gameObject)
    {
        mPatrolAI = patrolAI;
    }
    
    public override Type Tick()
    {
        if (mPatrolAI.isDead)
            return null;
        if(mPatrolAI.mNavMeshAgent == null || mPatrolAI.mNavMeshAgent.enabled == false)
            return null;
        mPatrolAI.mNavMeshAgent.destination = mPatrolAI.mTarget.position;
        
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
            if (mPatrolAI)
            {
                mPatrolAI.mNavMeshAgent.destination = mPatrolAI.mTarget.position;
                //trick:感觉这里给敌人设置不同的速度等参数会更好，不然召唤出来之后他们的逻辑都是完全一样的
                mPatrolAI.mNavMeshAgent.speed = Random.Range(1, mPatrolAI.enemy._RogueEnemyChaseMaxSpeed());
                mPatrolAI.mNavMeshAgent.acceleration = Random.Range(2, mPatrolAI.chaseMaxAcceleration);
            }
        }
    }
    
    //enter
    public override void OnStateEnter()
    {
        if (mPatrolAI.chaseType == RogueEnemyChaseType.DontMove)
        {
            return;
        }
        if (mPatrolAI.chaseType == RogueEnemyChaseType.ShootSpecialMuzzleDontMove)
        {
            mPatrolAI.StartCoroutine(mPatrolAI.ShootSpecialBulletWithMuzzle());
            return;
        }
        mPatrolAI.mNavMeshAgent.enabled = true;
        mPatrolAI.curStateName = "chase";
        
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
        else if (mPatrolAI.chaseType == RogueEnemyChaseType.ChaseAndShootSpecial)
        {
            string funcName = mPatrolAI.enemy.RogueEnemyChaseShootFunc;
            //startCorotine
            mPatrolAI.StartCoroutine(funcName);
        }
        else if (mPatrolAI.chaseType == RogueEnemyChaseType.ChaseAndShootWithSpecialMuzzle)
        {
            Debug.Log("ChaseAndShootWithSpecialMuzzle");
            //进到这里的逻辑都是直接Instantiate对应的muzzle，然后间隔一段时间shoot一次
            //string funcName = mPatrolAI.enemy.RogueEnemyChaseShootFunc; //e.g.ShootBulletMuzzleSpiral
            //GameObject muzzle = Addressables.LoadAssetAsync<GameObject>(funcName).WaitForCompletion();
            mPatrolAI.StartCoroutine(mPatrolAI.ShootSpecialBulletWithMuzzle());
        }
        else if (mPatrolAI.chaseType == RogueEnemyChaseType.AddSthToPlayer)
        {
            string funcName = mPatrolAI.enemy.RogueEnemyChaseShootFunc;
            
        }
    }
}
