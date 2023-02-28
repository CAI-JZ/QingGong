using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : MovementBaseState
{
    public JumpState(StateMachine stateMachine, StateFactory factory) : base(stateMachine, factory, "Jump") { }

    public override void Enter()
    {
        base.Enter();
        HandleJump();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (_controller.currentVelY < 0)
        {
            _controller.SwitchState(_moveFactory.Fall());
        }
        if (_controller.CanWallRun)
        {
            _controller.SwitchState(_moveFactory.WallRun());
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _controller.SwitchState(_moveFactory.Dash());
        }

        if (_controller.InputDir.x != 0 && _controller.currentVelX == 0)
        {
            float dirMul = _controller.InputDir.x >= 0 ? _controller.FallHorizontalMul : _controller.FallHorizontalMul * -1f;
            _controller.currentVelX += _controller.moveAcceleration * dirMul * Time.deltaTime;
            _controller.currentVelX = Mathf.Clamp(_controller.currentVelX, _controller.MaxSpeed * -1, _controller.MaxSpeed);
        }
    }


    public override void Exit()
    {
        base.Exit();
        //when exit
        //stop animator;
    }

    private void HandleJump()
    {
        Debug.Log("Handle Jump");
        _controller.currentVelY = _controller.JumpHight;
    }

}
