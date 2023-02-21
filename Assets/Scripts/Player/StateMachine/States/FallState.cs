using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : MovementBaseState
{
    public FallState(StateMachine stateMachine, StateFactory facotry) : base(stateMachine, facotry, "Fall") {}

    public override void Enter()
    {
        base.Enter();   
        
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (_controller.InputDir.x != 0 && _controller.currentVelX == 0)
        {
            float dirMul = _controller.InputDir.x >= 0 ? _controller.FallHorizontalMul : _controller.FallHorizontalMul *-1f;
            _controller.currentVelX += _controller.moveAcceleration * dirMul * Time.deltaTime;
            _controller.currentVelX = Mathf.Clamp(_controller.currentVelX, _controller.MaxSpeed * -1, _controller.MaxSpeed);
        }
        if (_controller.IsGrounded)
        {
            BaseState newstate = _controller.velocity.x == 0 ? _moveFactory.Idle() : _moveFactory.Run();
            _controller.SwitchState(newstate);
        }
        if (_controller.CanJump)
        {
            _controller.DebugLog("¥”fallState«–ªª");
            _controller.SwitchState(_moveFactory.Jump());
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _controller.SwitchState(_moveFactory.Dash());
        }
    }

    public override void Exit()
    {
        
    }
}
