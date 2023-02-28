using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumpState : MovementBaseState
{
    public WallJumpState(StateMachine stateMachine, StateFactory factory) : base(stateMachine, factory, "Wall Jump") { }

    public override void Enter()
    {
        base.Enter();
        _controller.SwitchState(_moveFactory.Jump());
        
    }

    public override void UpdateState()
    {
        base.UpdateState();
        //if()
    }

    public override void Exit()
    {
        base.Exit();
        HandleWallJump();
    }

    private void HandleWallJump()
    {
        _controller.ResetVelocity();
        _controller.currentVelX = _controller.WallRef.normal.x * _controller.MaxSpeed;
        Debug.Log("WallJumpX :"+_controller.currentVelX);
        //_controller.currentVelY = _controller.JumpHight;
        Debug.Log("改了速度");
        _controller.playerDir = _controller.playerDir == PlayerDir.Right ? PlayerDir.Left : PlayerDir.Right;
        
        
    }
}
