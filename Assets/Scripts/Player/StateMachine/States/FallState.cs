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
        if (_controller.IsGrounded)
        {
            BaseState newstate = _controller.velocity.x != 0 ? _moveFactory.Idle() : _moveFactory.Run();
            _controller.SwitchState(newstate);
        }
    }

    public override void Exit()
    {
        
    }
}
