using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class HPlayerIdleState : HPlayerBaseState
{
    public HPlayerIdleState(HPlayerStateMachine currentContext, HPlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        
    }
    public override void EnterState()
    {
        if (!_ctx.IsInThirdPersonCamera)
        {
            _ctx.Animator.SetBool(_ctx.IsWalkingHash, false);
            _ctx.Animator.SetBool(_ctx.IsRunningHash, false);
        }
        _ctx.AppliedMovementX = 0;
        _ctx.AppliedMovementZ = 0;
        HAudioManager.instance.Stop(_ctx.gameObject);
    }

    public override void InitializeSubState()
    {
        
    }

    public override void ExitState()
    {
        
    }

    public override void CheckSwitchStates()
    {
        if (_ctx.IsMovementPressed && _ctx.IsRunPressed)
        {
            SwitchState(_factory.Run());
        } else if (_ctx.IsMovementPressed)
        {
            SwitchState(_factory.Walk());
        }
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }
}
