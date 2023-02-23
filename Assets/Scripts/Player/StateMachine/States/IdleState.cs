using System.Collections;
using UnityEngine;

public class IdleState : GroundedState
{

    public IdleState(StateMachine stateMachine, StateFactory factory) : base(stateMachine, factory, "Idle") { }

    public override void Enter()
    {
        base.Enter();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (_controller.InputDir.x != 0)
        {
            _controller.SwitchState(_moveFactory.Run());
        }
        if (_controller.currentVelX != 0)
        { 
            _controller.currentVelX = Mathf.MoveTowards(_controller.currentVelX, 0, _controller.deAcceleration * Time.deltaTime);
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
