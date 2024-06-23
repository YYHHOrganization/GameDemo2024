using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YRuanmeiPet : YPetBase
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
    //test
    private void Update()
    {
        // testrm();
    }
}
