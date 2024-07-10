using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HPlayerWithWaterState : HPlayerBaseState, IRootState
{
    public HPlayerWithWaterState(HPlayerStateMachine currentContext, HPlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        _isRootState = true;
    }

    public void HandleGravity()
    {
        // _ctx.CurrentMovementY = -50f;  //在水中不受重力的影响
        // _ctx.AppliedMovementY = -50f; 
        // //_ctx.CurrentMovementY = -0.5f;
        //这里应该强制让角色往下游一段，并且不能切换状态
    }

    private bool couldFloatUp = true;
    public override void EnterState()
    {
        _ctx.Animator.SetBool(_ctx.IsSwimmingHash, true);
        _ctx.SetInWater(true);
        InitializeSubState();
        HandleGravity();
        couldFloatUp = false;
        DOVirtual.DelayedCall(1f, () => { couldFloatUp = true; });
    }

    public override void InitializeSubState()
    {
        if (!_ctx.IsMovementPressed && !_ctx.IsRunPressed)
        {
            SetSubState(_factory.SwimIdle());
        } else if(_ctx.IsMovementPressed && !_ctx.IsRunPressed)
        {
            SetSubState(_factory.SwimSlow());
        } else 
        {
            SetSubState(_factory.SwimFast());
        }
    }

    public override void ExitState()
    {
        _ctx.Animator.SetBool(_ctx.IsSwimmingHash, false);
        _ctx.SetInWater(false);
    }

    public override void CheckSwitchStates()
    {
        if (!_ctx.IsInWater && _ctx.CharacterController.isGrounded)
        {
            SwitchState(_factory.Grounded());
        }
        else if (_ctx.IsDie)
        {
            SwitchState(_factory.Die());
        }
        if (!couldFloatUp) return;
        else if (_ctx.IsFloatOnWater)
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
