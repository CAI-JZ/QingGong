using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunState : MovementBaseState
{
    public WallRunState(StateMachine stateMachine, StateFactory factory) : base(stateMachine, factory, "Wall Run") { }

    public override void Enter()
    {
        base.Enter();
        _controller.ResetVelocity();
        _controller.currentVelX = 0;
        //_controller.UseGravity = false;

    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (CheckDistanceY() > -0.1f)
        {
            _controller.SwitchState(_moveFactory.Idle());
        }
        if (_controller.InputDir.x == 0 || !_controller.CanWallRun)
        {
            _controller.SwitchState(_moveFactory.Fall());
        }
   
        if (PlayerInput._instance.jumpBtnDown)
        {
            _controller.SwitchState(_moveFactory.WallJump());
        }
        if (_controller.InputDir.x != 0 )
        {
            HandleWallRun();
        }

        
    }

    public override void Exit()
    {
        base.Exit();
        _controller.UseGravity = true;
        //_controller.ResetVelocity();
    }

    private void HandleWallRun()
    {
        _controller.currentVelY = _controller.MaxSpeed;
        _controller.currentVelX = 0;

    }

    private float CheckDistanceY()
    {
        if (!_controller.WallRef)
        {
            return -999;
        }
        float checkPointY = _controller.WallRef.collider.transform.position.y + _controller.WallRef.collider.bounds.size.y / 2 + 0.2f;
        float distanceY = _controller.transform.position.y - checkPointY;
        return distanceY;
    }

    private void AutoJump() 
    {
            
    }

  
}
