using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunState : MovementBaseState
{
    public WallRunState(StateMachine stateMachine, StateFactory factory) : base(stateMachine, factory, "Wall Run") {}

    public override void Enter()
    {
        base.Enter();
        _controller.UseGravity = false;
    }

    public override void UpdateState()
    {
        base.UpdateState();
        //if (!_controller.IsTouchWall)
        //{
        //    _controller.SwitchState(_moveFactory.Idle());
        //}
        if (_controller.InputDir.x != 0)
        {
            HandleWallRun();
        }
        else
        {
            _controller.SwitchState(_moveFactory.Fall());
        }
        if (PlayerInput._instance.jumpBtnDown)
        {
            _controller.SwitchState(_moveFactory.WallJump());
        }
        
    }

    public override void Exit()
    {
        base.Exit();
        _controller.UseGravity = true;
    }

    private void HandleWallRun()
    {
        _controller.currentVelY = _controller.MaxSpeed;
        _controller.currentVelX = 0;

    }
}
