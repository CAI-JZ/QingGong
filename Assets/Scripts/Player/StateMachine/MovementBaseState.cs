using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementBaseState : BaseState
{
    protected PlayerController _controller;
    protected MovementStateFactory _moveFactory;
    protected string name;

    public MovementBaseState(StateMachine stateMachine, StateFactory factory, string name) : base(stateMachine, factory, name)
    {
        _controller = (PlayerController)stateMachine;
        _moveFactory = (MovementStateFactory)factory;
        this.name = name;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("½øÈë:" + name);
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
