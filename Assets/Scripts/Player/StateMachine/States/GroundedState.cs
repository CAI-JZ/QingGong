using UnityEngine;

public class GroundedState : MovementBaseState
{
    public GroundedState(StateMachine stateMachine, StateFactory factory,string name) : base(stateMachine, factory, name) {}

    public override void Enter()
    {
        base.Enter();
        _controller.currentVelY = 0;
        // set character on the ground;
        if (_controller.GroundRef != null)
        {
            _controller.transform.position = _controller._charMove.downInfo.point + new Vector3(0, 0.9f, 0);
        }

        // Reset Jump varibles;
        _controller.IsJumpEarlyUp = false;
        _controller.CoyoteJumpTimer = _controller.CoyoteJump;
    }

    public override void UpdateState()
    {
        base.UpdateState();

        // Switch to Jump
        if (_controller.CanJump)
        {
            _controller.SwitchState(_moveFactory.Jump());
        }

        // Switch to Fall
        if (_controller.currentVelY < 0) 
        {
            _controller.SwitchState(_moveFactory.Fall());
        }
    }
    public override void Exit()
    {
        
    }
}
