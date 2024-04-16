using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HRogueEnemyCommonWanderState : HRogueEnemyBaseState
{
    //一般的巡逻状态，希望可以配置，比如移动的速度，移动的范围，怪物的移动是勤奋的/懒散的，是否可能发射子弹等等,是否需要寻路系统等
    private HRogueEnemyPatrolAI mPatrolAI;
    private readonly LayerMask mlayerMask;
    private LayerMask wallLayer;
    
    public HRogueEnemyCommonWanderState(HRogueEnemyPatrolAI patrolAI) : base(patrolAI.gameObject)
    {
        mPatrolAI = patrolAI;
        wallLayer= patrolAI.mlayer;
        mlayerMask = patrolAI.mlayer;
    }

    //闲逛时候的Tick逻辑
    public override Type Tick()
    {
        if (!mPatrolAI.mTarget)
        {
            if (YPlayModeController.Instance.curCharacter) //尝试Get一下当前的target
            {
                mPatrolAI.setTarget(YPlayModeController.Instance.curCharacter.transform);
            }
        }

        if (mPatrolAI.mTarget && mPatrolAI.CheckIfTargetIsNear(mPatrolAI.enemy._RogueWanderPlayerSensitivePlayerDis()) || goToChase)
        {
            mPatrolAI.StopAllCoroutines();
            return typeof(HRogueEnemyCommonChaseState);
        }
        
        if (mPatrolAI.isDead)
            return null;

        return null;
    }
    
    
    //暂时未使用，后面可能会有让怪物互相攻击的道具，可能需要调用这个函数
    private void SetChaseTarget(Transform target)
    {
        mPatrolAI.setTarget(target);
    }


    private bool goToChase = false;
    
    public override void OnStateEnter()
    {
        mPatrolAI.curStateName = "wander";
        // 进入WanderState之后，就可以根据一些敌人类型来做处理了
        if(mPatrolAI.wanderType == RogueEnemyWanderType.ShootBulletForwardWithoutMove)
        {
            mPatrolAI.ShootBulletForward(); 
        }
        else if (mPatrolAI.wanderType == RogueEnemyWanderType.ShootBulletForwardWithMove)
        {
            mPatrolAI.ShootBulletForward();
            mPatrolAI.EnemyMoveRandomly();
        }
        else if (mPatrolAI.wanderType == RogueEnemyWanderType.MoveRandomlyWithStop)
        {
            mPatrolAI.EnemyMoveRandomly();
        }
        else if (mPatrolAI.wanderType == RogueEnemyWanderType.JustToChaseState)
        {
            goToChase = true;
        }
        else if (mPatrolAI.wanderType == RogueEnemyWanderType.RandomJump)
        {
            //random jump coroutine
            mPatrolAI.StartCoroutine(mPatrolAI.RandomJump());
        }
        
    }
    
    
}
