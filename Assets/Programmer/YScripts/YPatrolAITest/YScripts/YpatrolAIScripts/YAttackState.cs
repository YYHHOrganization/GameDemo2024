using System;
using System.Collections;
using System.Collections.Generic;
// using Unity.VisualScripting;
using UnityEngine;

public class YAttackState : YBaseState
{
    private YPatrolAI patrolAI;
    private float tempAttackReadyTimer;
    public YAttackState(YPatrolAI yPatrolAI):base(yPatrolAI.gameObject)
    {
        patrolAI = yPatrolAI;
        tempAttackReadyTimer = 0f;
    }
    public override Type Tick()
    {
        if(patrolAI.mTarget==null)
        {
            return typeof(YWanderState);
        }
        //Debug.Log("Atack");
        //patrolAI.AttackFunc();
        //patrolAI.animator.SetInteger("AnimState", 1);
        //return typeof(YChaseState);

        //可能不能这样 这样的话就可能会出现攻击就一帧 

        tempAttackReadyTimer -= Time.deltaTime;
        if (tempAttackReadyTimer <= 0f)
        {
            Debug.Log("Atack");
            //YUIManager.flashScreen();

            //attack的时候向着角色
            transform.LookAt(patrolAI.mTarget);
            patrolAI.AttackFunc();
            tempAttackReadyTimer = YGameSetting.attackReadyTimer;
            //如果攻击结束
            //且目标距离自己已经超出攻击范围 此处 在追逐范围内和不在都暂时直接让他返回wanderStater
            if (Vector3.Distance(transform.position, patrolAI.mTarget.transform.position) > YGameSetting.AttackRange)
            {
                patrolAI.animator.SetInteger("AnimState", 1);
                return typeof(YChaseState);
            }
        }
        // 如果攻击结束
        // 且目标距离自己已经超出攻击范围 此处 在追逐范围内和不在都暂时直接让他返回wanderStater
        //if (Vector3.Distance(transform.position, patrolAI.mTarget.transform.position) > YGameSetting.AttackRange)
        //{
        //    return typeof(YWanderState);
        //}

        //// 如果攻击结束
        //// 且目标距离自己已经超出攻击范围 此处 在追逐范围内和不在都暂时直接让他返回wanderStater
        //if (tempAttackReadyTimer <= 0f
        //    && Vector3.Distance(transform.position, patrolAI.mTarget.transform.position) > YGameSetting.AttackRange)
        //{
        //    tempAttackReadyTimer = YGameSetting.attackReadyTimer;
        //    return typeof(YWanderState);
        //}

        if (patrolAI.isDead == true)
        {
            return typeof(YDieState);
        }
        return null;
    }

}
