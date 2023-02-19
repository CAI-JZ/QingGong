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
        
    }

    public override void UpdateState()
    {
        if (_controller.IsGrounded)
        {
            _controller.SwitchState(_moveFactory.Idle());
        }

    }

    public override void UpdatePhysic()
    {
        base.UpdatePhysic();

    }

    public override void Exit()
    {
        
    }
}
