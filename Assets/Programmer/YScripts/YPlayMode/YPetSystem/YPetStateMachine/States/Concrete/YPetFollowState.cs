using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;

/// <summary>
/// 跟随状态 跟随当前角色
/// </summary>
public class YPetFollowState : YPetBaseState
{
    private YPetBase mPatrolAI;
    public YPetFollowState(YPetBase yPetBase) : base(yPetBase.gameObject)
    {
        mPatrolAI = yPetBase;
    }

    public override Type Tick()
    {
        //test
        //等待1s后切换到攻击状态
        return typeof(YPetAttackState);
        
        // if (mPatrolAI.isDead)
        //     return null;
        if(mPatrolAI.mNavMeshAgent.enabled == false)
            return null;
        mPatrolAI.mNavMeshAgent.destination = mPatrolAI.curCharacterTrans.position;
        
        //如果距离角色足够近且有敌人，那么就切换到攻击状态
        if(mPatrolAI.IsCloseEnoughToCharacter() &&mPatrolAI.CheckEnemyCount())
        {
            return typeof(YPetAttackState);
        }
        
        return null;
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
        Debug.Log("PET YPetFollowState OnStateEnter");
        //打开navmeshagent
        mPatrolAI.mNavMeshAgent.enabled = true;
        
        // if (mPatrolAI.chaseType == RogueEnemyChaseType.DontMove)
        // {
        //     return;
        // }
        // if (mPatrolAI.chaseType == RogueEnemyChaseType.ShootSpecialMuzzleDontMove)
        // {
        //     mPatrolAI.StartCoroutine(mPatrolAI.ShootSpecialBulletWithMuzzle());
        //     return;
        // }
        // mPatrolAI.mNavMeshAgent.enabled = true;
        // mPatrolAI.curStateName = "chase";
        //
        // //根据chase类型决定接下来的逻辑
        // if (mPatrolAI.chaseType == RogueEnemyChaseType.JustChase)
        // {
        //     SetNavmeshDestination();
        // }
        // else if (mPatrolAI.chaseType == RogueEnemyChaseType.ChaseAndShootPlayer)
        // {
        //     SetNavmeshDestination();
        //     mPatrolAI.ShootBulletForward(true,true);
        // }
        // else if (mPatrolAI.chaseType == RogueEnemyChaseType.ChaseAndShootSpecial)
        // {
        //     string funcName = mPatrolAI.enemy.RogueEnemyChaseShootFunc;
        //     //startCorotine
        //     mPatrolAI.StartCoroutine(funcName);
        // }
        // else if (mPatrolAI.chaseType == RogueEnemyChaseType.ChaseAndShootWithSpecialMuzzle)
        // {
        //     Debug.Log("ChaseAndShootWithSpecialMuzzle");
        //     //进到这里的逻辑都是直接Instantiate对应的muzzle，然后间隔一段时间shoot一次
        //     //string funcName = mPatrolAI.enemy.RogueEnemyChaseShootFunc; //e.g.ShootBulletMuzzleSpiral
        //     //GameObject muzzle = Addressables.LoadAssetAsync<GameObject>(funcName).WaitForCompletion();
        //     mPatrolAI.StartCoroutine(mPatrolAI.ShootSpecialBulletWithMuzzle());
        // }
        // else if (mPatrolAI.chaseType == RogueEnemyChaseType.AddSthToPlayer)
        // {
        //     string funcName = mPatrolAI.enemy.RogueEnemyChaseShootFunc;
        //     
        // }
    }
    
    
}
