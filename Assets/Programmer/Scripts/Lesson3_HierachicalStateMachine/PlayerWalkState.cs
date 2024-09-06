using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : HPlayerBaseState
{
    public PlayerWalkState(HPlayerStateMachine currentContext, HPlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        
    }
    public override void EnterState()
    {
        if (!_ctx.IsInThirdPersonCamera)
        {
            _ctx.Animator.SetBool(_ctx.IsWalkingHash, true);
            _ctx.Animator.SetBool(_ctx.IsRunningHash, false);
        }
        HAudioManager.instance.Play("FootStepOnGround", this._ctx.gameObject);
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
        else if(_ctx.IsMovementPressed && _ctx.IsRunPressed)
        {
            SwitchState(_factory.Run());
        }
    }

    public override void UpdateState()
    {
        _ctx.AppliedMovementX = _ctx.CurrentMovementInput.x * 2;
        _ctx.AppliedMovementZ = _ctx.CurrentMovementInput.y * 2;
        CheckSwitchStates();
    }
}
