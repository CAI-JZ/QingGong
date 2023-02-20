using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : BaseState
{
    private PlayerController _controller;
    private MovementStateFactory _moveFactory;

    public FallState(StateMachine stateMachine, StateFactory facotry) : base(stateMachine, facotry, "Fall")
    {
        _controller = (PlayerController)stateMachine;
        _moveFactory = (MovementStateFactory)facotry;
    }

    public override void Enter()
    {
        base.Enter();   
        
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (PlayerInput._instance.moveDir != Mathf.Epsilon && _controller.currentVelocityX == 0)
        {
            _controller.currentVelocityX += _controller.moveAcceleration * Time.deltaTime;
            _controller.currentVelocityX = Mathf.Clamp(_controller.currentVelocityX, 0, _controller.MaxSpeed);
        }
        if (_controller.IsGrounded)
        {
            BaseState newstate = _controller.velocity.x == 0 ? _moveFactory.Idle() : _moveFactory.Run();
            _controller.SwitchState(newstate);
        }
        if (_controller.CanJump)
        {
            _controller.DebugLog("¥”fallState«–ªª");
            //_controller.SwitchState(_moveFactory.Jump());
        }
    }

    public override void Exit()
    {
        
    }
}
