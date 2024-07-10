using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPlayerSwimFastState : HPlayerBaseState
{
    public HPlayerSwimFastState(HPlayerStateMachine currentContext, HPlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        
    }
    public override void EnterState()
    {
        if (!_ctx.IsInThirdPersonCamera)
        {
            _ctx.Animator.SetBool(_ctx.IsWalkingHash, true);
            _ctx.Animator.SetBool(_ctx.IsRunningHash, true);
        }
        
        //HAudioManager.instance.Play("RunFootStepOnGround", _ctx.gameObject);
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
        else if(!_ctx.IsRunPressed && _ctx.IsMovementPressed)
        {
            SwitchState(_factory.SwimSlow());
        }
    }

    public override void UpdateState()
    {
        _ctx.AppliedMovementX = _ctx.CurrentMovementInput.x * _ctx.RunMultiplier;
        _ctx.AppliedMovementZ = _ctx.CurrentMovementInput.y * _ctx.RunMultiplier;
        CheckSwitchStates();
    }
}
