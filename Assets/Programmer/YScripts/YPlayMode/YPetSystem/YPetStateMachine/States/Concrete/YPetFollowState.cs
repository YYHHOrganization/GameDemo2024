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
        if(mPatrolAI.mNavMeshAgent.enabled == false)
            return null;
        mPatrolAI.mNavMeshAgent.destination = mPatrolAI.curCharacterTrans.position;
        
        //如果距离角色足够近且有敌人，那么就切换到攻击状态
        if(mPatrolAI.attackType == PetAttackType.MeleeAttack)
        {
            //近战的话得追到怪物身边再打，或者边追边打？ //如果距离角色足够近且有敌人，那么就切换到攻击状态
            GameObject enemy = mPatrolAI.CheckAndGetEnemy();
            if(enemy!=null)
            {
                mPatrolAI.ChaseTarget = enemy.transform;
                // //mPatrolAI.mNavMeshAgent.destination = enemy.transform.position;
                // mPatrolAI.mNavMeshAgent.SetDestination(enemy.transform.position);
                return typeof(YPetChaseAndAttackEnemyState);
            }
        }
        //远战 直接攻击（射击子弹就好）
        else if(mPatrolAI.attackType == PetAttackType.RangedAttack)
        {
            if(mPatrolAI.IsCloseEnoughToCharacter() &&mPatrolAI.CheckEnemyCount())
            {
                return typeof(YPetAttackState);
            }
        }
        else if(mPatrolAI.attackType == PetAttackType.ChaseBeforeMeleeAttack)
        {
            //近战的话得追到怪物身边再打，或者边追边打？ //如果距离角色足够近且有敌人，那么就切换到攻击状态
            GameObject enemy = mPatrolAI.CheckAndGetEnemy();
            if(enemy!=null)
            {
                mPatrolAI.ChaseTarget = enemy.transform;
                // //mPatrolAI.mNavMeshAgent.destination = enemy.transform.position;
                // mPatrolAI.mNavMeshAgent.SetDestination(enemy.transform.position);
                return typeof(YPetChaseState);
            }
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
        mPatrolAI.EnterFollowState();   
        //打开navmeshagent
        mPatrolAI.mNavMeshAgent.enabled = true;
        
    }
    
    
}
