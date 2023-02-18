using System.Collections;
using UnityEngine;

public class IdleState : GroundedState
{
    public IdleState(PlayerController stateMachine, MovementStateFactory factory) : base(stateMachine,factory,"Idle") { }


    public override void Enter()
    {
        _controller.DebugLog("Idle");

    }

    public override void UpdateState()
    {
        //deacceleration  when not input
        if (_controller.velocity.x != 0)
        {
            _controller.velocity.x = Mathf.MoveTowards(_controller.velocity.x, 0, _controller.deAcceleration * Time.deltaTime);
        }
        if (_controller.InputDir != 0)
        {
            _controller.SwitchState(_moveFactory.Run());
        }

        
       
    }

    public override void Exit()
    {
        
    }

}
