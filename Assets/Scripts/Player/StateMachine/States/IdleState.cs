using System.Collections;
using UnityEngine;

public class IdleState : GroundedState
{
    public IdleState(PlayerController stateMachine, MovementStateFactory factory) : base(stateMachine,factory,"Idle") { }


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
    }

    public override void UpdatePhysic()
    {
        base.UpdatePhysic();
        //deacceleration  when not input
        if (_controller.velocity.x != 0)
        {
            _controller.velocity.x = Mathf.MoveTowards(_controller.velocity.x, 0, _controller.deAcceleration * Time.deltaTime);
        }
    }

    public override void Exit()
    {
        
    }

}
