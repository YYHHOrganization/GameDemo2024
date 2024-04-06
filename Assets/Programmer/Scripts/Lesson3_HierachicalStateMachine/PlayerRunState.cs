using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : HPlayerBaseState
{
    public PlayerRunState(HPlayerStateMachine currentContext, HPlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        
    }
    public override void EnterState()
    {
        if (!_ctx.IsInThirdPersonCamera)
        {
            _ctx.Animator.SetBool(_ctx.IsWalkingHash, true);
            _ctx.Animator.SetBool(_ctx.IsRunningHash, true);
        }
        
        HAudioManager.instance.Play("RunFootStepOnGround", _ctx.gameObject);
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
            SwitchState(_factory.Idle());
        }
        else if(!_ctx.IsRunPressed && _ctx.IsMovementPressed)
        {
            SwitchState(_factory.Walk());
        }
    }

    public override void UpdateState()
    {
        _ctx.AppliedMovementX = _ctx.CurrentMovementInput.x * _ctx.RunMultiplier;
        _ctx.AppliedMovementZ = _ctx.CurrentMovementInput.y * _ctx.RunMultiplier;
        CheckSwitchStates();
    }
}
