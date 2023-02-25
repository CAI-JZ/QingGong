using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumpState : MovementBaseState
{
    public WallJumpState(StateMachine stateMachine, StateFactory factory) : base(stateMachine, factory, "Wall Jump") { }

    bool isWallJump;

    public override void Enter()
    {
        base.Enter();
        isWallJump = true;
        HandleWallJump();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if(!isWallJump)
            _controller.SwitchState(_moveFactory.Jump());
    }

    public override void Exit()
    {
        base.Exit();
    }

    private void HandleWallJump()
    {
        RaycastHit2D wall = _controller.WallRef;
        //_controller.SwitchState(_moveFactory.Jump());
        _controller.currentVelX = wall.normal.x * _controller.MaxSpeed;
        Debug.Log("改了速度");
        _controller.playerDir = _controller.playerDir == PlayerDir.Right ? PlayerDir.Left : PlayerDir.Right;
        isWallJump = false;
        
    }
}
