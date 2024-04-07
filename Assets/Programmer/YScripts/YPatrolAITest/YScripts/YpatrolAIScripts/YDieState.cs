using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YDieState : YBaseState
{
    private YPatrolAI patrolAI;
    public YDieState(YPatrolAI yPatrolAI) :base(yPatrolAI.gameObject)
    {
        patrolAI = yPatrolAI;
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        patrolAI.animator.SetInteger("AnimState", 3);
        //patrolAI.mNavMeshAgent.ResetPath();
        patrolAI.mNavMeshAgent.enabled = false;
        if (patrolAI.SpotLightWander) patrolAI.SpotLightWander.SetActive(false);
        if (patrolAI.SpotLightChase) patrolAI.SpotLightChase.SetActive(false);    
        
        GameObject mesh = patrolAI.Mesh;
        DissolvingControllery dissolving = mesh.GetComponent<DissolvingControllery>();
        dissolving.SetMaterialsPropAndBeginDissolve(mesh,1f);
        
        //瓦解
        patrolAI.DisintegrateDissolveVFX.SetActive(true);
        
        //爆炸
        patrolAI.DieExplosionEff.SetActive(true);
    }

    public override Type Tick()
    {
        return null;
    }
}
