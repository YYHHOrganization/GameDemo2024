using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YPlayerDieState : HPlayerBaseState, IRootState
{
    public YPlayerDieState(HPlayerStateMachine currentContext, HPlayerStateFactory playerStateFactory) : base(
        currentContext, playerStateFactory)
    {
        _isRootState = true;
    }
    public override void EnterState()
    {
        InitializeSubState();
        //Debug.Log("now in fall state");
        _ctx.Animator.SetBool("isDie", true);
    }

    public override void UpdateState()
    { 
        HandleGravity();
        // CheckSwitchStates();
        
        //冻结输入，且无法移动
        FrozenInput();
    }

    public void HandleGravity()
    {
        float previousYVelocity = _ctx.CurrentMovementY;
        _ctx.CurrentMovementY += _ctx.Gravity * Time.deltaTime;
        _ctx.AppliedMovementY = Mathf.Max((previousYVelocity + _ctx.CurrentMovementY) * 0.5f, -20.0f);
    }

    public void FrozenInput()
    {
        _ctx.SetInputActionDisableOrEnable(true);
        _ctx.AppliedMovementX = 0;
        _ctx.AppliedMovementZ = 0;
    }

    public override void ExitState()
    {
        // _ctx.Animator.SetBool("isFalling", false);
    }

    public override void CheckSwitchStates()
    {
        // if(_ctx.CharacterController.isGrounded)
        // {
        //     SwitchState(_factory.Grounded());
        // }
    }

    public override void InitializeSubState()
    {
        // if (!_ctx.IsMovementPressed && !_ctx.IsRunPressed)
        // {
        //     SetSubState(_factory.Idle());
        // } else if(_ctx.IsMovementPressed && !_ctx.IsRunPressed)
        // {
        //     SetSubState(_factory.Walk());
        // } else 
        // {
        //     SetSubState(_factory.Run());
        // }
    }
}
