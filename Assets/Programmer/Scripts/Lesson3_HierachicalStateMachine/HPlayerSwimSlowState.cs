using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPlayerSwimSlowState : HPlayerBaseState
{
    public HPlayerSwimSlowState(HPlayerStateMachine currentContext, HPlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        
    }
    public override void EnterState()
    {
        if (!_ctx.IsInThirdPersonCamera)
        {
            _ctx.Animator.SetBool(_ctx.IsWalkingHash, true);
            _ctx.Animator.SetBool(_ctx.IsRunningHash, false);
        }
    }

    public override void InitializeSubState()
    {
        
    }

    public override void ExitState()
    {
        
    }

    public override void CheckSwitchStates()
    {
        if(!_ctx.IsMovementPressed)
        {
            SwitchState(_factory.SwimIdle());
        }
        else if(_ctx.IsMovementPressed && _ctx.IsRunPressed)
        {
            SwitchState(_factory.SwimFast());
        }
    }

    public override void UpdateState()
    {
        _ctx.AppliedMovementX = _ctx.CurrentMovementInput.x;
        _ctx.AppliedMovementZ = _ctx.CurrentMovementInput.y;
        CheckSwitchStates();
    }
}
