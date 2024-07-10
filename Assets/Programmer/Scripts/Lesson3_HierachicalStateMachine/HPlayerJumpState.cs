using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HPlayerJumpState : HPlayerBaseState, IRootState
{
    public HPlayerJumpState(HPlayerStateMachine currentContext, HPlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        _isRootState = true;
        
    }
    public override void EnterState()
    {
        InitializeSubState();
        HandleJump();
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

    IEnumerator jumpResetRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        _ctx.JumpCount = 0;
    }
    
    public override void ExitState()
    {
        _ctx.Animator.SetBool(_ctx.IsJumpingHash, false);
        if (_ctx.IsJumpPressed)
        {
            _ctx.RequireNewJumpPress = true;
        }
        //_ctx.IsJumpAnimating = false;
        _ctx.CurrentJumpResetRoutine = _ctx.StartCoroutine(jumpResetRoutine());
        if (_ctx.JumpCount == 3)
        {
            _ctx.JumpCount = 0;
            _ctx.Animator.SetInteger(_ctx.JumpCountHash, _ctx.JumpCount);
        }
    }

    public override void CheckSwitchStates()
    {
        if (_ctx.CharacterController.isGrounded)
        {
            SwitchState(_factory.Grounded());
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

    public override void UpdateState()
    {
        HandleGravity();
        CheckSwitchStates();  //CheckSwitchStates方法要放在Update的最后，因为我们需要先处理完重力，再检查是否需要切换状态
        //其他的也是类似，CheckSwitchStates要放在Update的最后
    }

    void HandleJump()
    {
        if (_ctx.IsInWater) return; //在水中的时候不能jump
        if (_ctx.JumpCount <= 3 && _ctx.CurrentJumpResetRoutine != null)
        {
            _ctx.StopCoroutine(_ctx.CurrentJumpResetRoutine);
        }
        _ctx.Animator.SetBool(_ctx.IsJumpingHash, true);
        _ctx.IsJumping = true;
        _ctx.JumpCount += 1;
        string jumpAudio = "JumpAudio" + _ctx.JumpCount;
        HAudioManager.instance.Play(jumpAudio, _ctx.gameObject);
        
        _ctx.Animator.SetInteger(_ctx.JumpCountHash, _ctx.JumpCount);
        _ctx.CurrentMovementY = _ctx.InitialJumpVelocities[_ctx.JumpCount];
        _ctx.AppliedMovementY = _ctx.InitialJumpVelocities[_ctx.JumpCount];
    }

    public void HandleGravity()
    {
        bool isFalling = _ctx.CurrentMovementY <= 0.0f || !_ctx.IsJumpPressed;
        float fallMultiplier = 2.0f;

        if (isFalling)
        {
            float previousYVelocity = _ctx.CurrentMovementY;
            _ctx.CurrentMovementY = _ctx.CurrentMovementY + (_ctx.JumpGravities[_ctx.JumpCount] * fallMultiplier * Time.deltaTime);
            _ctx.AppliedMovementY = Mathf.Max((previousYVelocity + _ctx.CurrentMovementY) * 0.5f, -20.0f);
        }
        else
        {
            float previousYVelocity = _ctx.CurrentMovementY;
            _ctx.CurrentMovementY = _ctx.CurrentMovementY + (_ctx.JumpGravities[_ctx.JumpCount] * Time.deltaTime);
            _ctx.AppliedMovementY = (previousYVelocity + _ctx.CurrentMovementY) * 0.5f;
        }
    }
}
