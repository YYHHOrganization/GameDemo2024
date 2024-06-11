using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class YMarch7thPet :YPetBase
{
    
    protected override void InitStateMachine()
    {
        var states = new Dictionary<Type, YPetBaseState>
        {
            { typeof(YPetFollowState), new YPetFollowState(this) },
            { typeof(YPetChaseState), new YPetChaseState(this) },
            { typeof(YPetAttackState), new YPetAttackState(this) }
        };
        GetComponent<YPetStateMachine>().SetStates(states);
    }
    
    
    public override void MuzzleShoot()
    {
        base.MuzzleShoot();
        
    }
    public override void MuzzleStopShoot()
    {
        base.MuzzleStopShoot();
    }
}
