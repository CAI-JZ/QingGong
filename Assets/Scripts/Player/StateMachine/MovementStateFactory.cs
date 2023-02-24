using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStateFactory : StateFactory
{
    public PlayerController _controller;

    public MovementStateFactory(StateMachine stateMachine) : base(stateMachine)
    {
        _controller = (PlayerController)stateMachine;
    }

    public BaseState Idle() { return new IdleState(_controller, this); }
    public BaseState Run() { return new RunState(_controller, this); }
    public BaseState Jump() { return new JumpState(_controller, this); }
    public BaseState Fall() { return new FallState(_controller, this); }
    public BaseState Dash() { return new DashState (_controller, this); }
    public BaseState WallRun() { return new WallRunState(_controller, this); }
    public BaseState WallJump() { return new WallJumpState(_controller, this); }
}
