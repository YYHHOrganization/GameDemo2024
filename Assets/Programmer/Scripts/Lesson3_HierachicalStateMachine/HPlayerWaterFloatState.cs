using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPlayerWaterFloatState : HPlayerBaseState, IRootState
{
    public HPlayerWaterFloatState(HPlayerStateMachine currentContext, HPlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        _isRootState = true;
    }

    public void HandleGravity()
    {
        _ctx.CurrentMovementY = 0;  //在水中不受重力的影响
        _ctx.AppliedMovementY = 0;
        //_ctx.CurrentMovementY = -0.5f;
    }
    
    
    public override void EnterState()
    {
        _ctx.Animator.SetBool(_ctx.IsSwimmingHash, true);
        InitializeSubState();
        HandleGravity();
        HandleUIShow(true);
    }
    
    private void HandleUIShow(bool isActive)
    {
        //显示潜水对应的UI
        HMessageShowMgr.Instance.ShowMessageOnOrOff("SHOW_KEYBOARD_CTRL", isActive);
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
        _ctx.SetOnWaterFloat(false);
        HandleUIShow(false);
    }

    public override void CheckSwitchStates()
    {
        if (!_ctx.IsFloatOnWater && _ctx.CharacterController.isGrounded)
        {
            SwitchState(_factory.Grounded());
        }
        else if (_ctx.IsDie)
        {
            SwitchState(_factory.Die());
        }
        else if (_ctx.IsDiveIntoWaterPress)
        {
            SwitchState(_factory.WithWater());
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