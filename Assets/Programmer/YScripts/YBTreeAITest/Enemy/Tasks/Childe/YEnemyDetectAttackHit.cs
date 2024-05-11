using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using Core.AI;
using UnityEngine;

public class YEnemyDetectAttackHit : YBTEnemyConditional
{
    private bool isGettingHit;
    public bool waitForHit = false;
    public override void OnAwake()
    {
        base.OnAwake();
        enemyBT.OnGettingHit += OnHit;
    }
    public override void OnStart()
    {
        if(waitForHit)
            isGettingHit = false;
    }
    void OnHit()
    {
        isGettingHit = true;
    }

    public override TaskStatus OnUpdate()
    {
        var returnTypeNegative = waitForHit ? TaskStatus.Running : TaskStatus.Failure;
        return isGettingHit ? TaskStatus.Success : returnTypeNegative;
    }

    public override void OnEnd()
    {
        isGettingHit = false;
    }
}
