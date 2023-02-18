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
        //_ctx.rb.mass = 30;
    }

    public override void UpdateState()
    {
        
    }

    public override void Exit()
    {
        
    }

   
    private void HanldeGravity()
    { 
        
    }
}
