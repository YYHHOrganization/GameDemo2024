using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPlayerGroundedState : HPlayerBaseState, IRootState
{
    public HPlayerGroundedState(HPlayerStateMachine currentContext, HPlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        _isRootState = true;
    }

    public void HandleGravity()
    {
        _ctx.CurrentMovementY = _ctx.Gravity;
        _ctx.AppliedMovementY = _ctx.Gravity;
    }
    public override void EnterState()
    {
        InitializeSubState();
        HandleGravity();
    }

    public override void InitializeSubState()
    {
        if (!_ctx.IsMovementPressed && !_ctx.IsRunPressed)
        {
            SetSubState(_factory.Idle());
        } else if(_ctx.IsMovementPressed && !_ctx.IsRunPressed)
        {
            SetSubState(_factory.Walk());
        } else 
        {
            SetSubState(_factory.Run());
        }
    }

    public override void ExitState()
    {
        
    }

    public override void CheckSwitchStates()
    {
        if (_ctx.IsJumpPressed && !_ctx.RequireNewJumpPress)
        {
            SwitchState(_factory.Jump());
        }
        else if (!_ctx.CharacterController.isGrounded)
        {
            SwitchState(_factory.Fall());
        }
        else if (_ctx.IsDie)
        {
            SwitchState(_factory.Die());
        }
        else if(_ctx.IsFloatOnWater)
        {
            SwitchState(_factory.FloatOnWater());
        }
    }

    // private float gravity = -9.8f;
    // void HandleGravity()
    // {
    //     // 即使角色没有跳跃，他也可能不在地面上，比如爬上一个台阶并下来，所以即使是在Grounded的情况下我们也需要检查角色是否在地面上
    //     if (_ctx.CharacterController.isGrounded)
    //     {
    //         _ctx.CurrentMovementY = _ctx.GroundedGravity;
    //         _ctx.AppliedMovementY = _ctx.GroundedGravity;
    //     } else
    //     {
    //         _ctx.CurrentMovementY += gravity * Time.deltaTime;
    //         _ctx.AppliedMovementY = _ctx.CurrentMovementY;
    //     }
    // }

    public override void UpdateState()
    {
        CheckSwitchStates();
        //HandleGravity();
    }
}
