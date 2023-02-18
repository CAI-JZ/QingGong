using UnityEngine;

public class GroundedState : BaseState
{
    protected PlayerController _controller;
    protected MovementStateFactory _moveFactory;

    public GroundedState(StateMachine stateMachine, StateFactory factory, string name ) : base(stateMachine,factory, name)
    {
        _controller = (PlayerController)stateMachine;
        _moveFactory = (MovementStateFactory)factory;
    }

    public override void Enter()
    {

    }

    public override void UpdateState()
    {
  
    }

    public override void Exit()
    {
        
    }

}
