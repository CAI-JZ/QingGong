using System.Collections;
using UnityEngine;

public class IdleState : GroundedState
{

    public IdleState(StateMachine stateMachine, StateFactory factory) : base(stateMachine, factory, "Idle") { }


    public override void Enter()
    {
        base.Enter();
        _controller.DebugLog("Idle");

    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (_controller.InputDir != 0)
        {
            _controller.SwitchState(_moveFactory.Run());
        }
        if (_controller.currentVelocityX != 0)
        {
            _controller.currentVelocityX = Mathf.MoveTowards(_controller.currentVelocityX, 0, _controller.deAcceleration * Time.deltaTime);
        }
    }

    public override void Exit()
    {
        
    }

}
