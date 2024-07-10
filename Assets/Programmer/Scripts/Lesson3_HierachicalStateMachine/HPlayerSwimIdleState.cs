using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPlayerSwimIdleState : HPlayerBaseState
{
    public HPlayerSwimIdleState(HPlayerStateMachine currentContext, HPlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
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
            SwitchState(_factory.SwimFast());
        } else if (_ctx.IsMovementPressed)
        {
            SwitchState(_factory.SwimSlow());
        }
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }
}
