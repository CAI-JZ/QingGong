using UnityEngine;

public class GroundedState : MovementBaseState
{
    public GroundedState(StateMachine stateMachine, StateFactory factory,string name) : base(stateMachine, factory, name) {}

    public override void Enter()
    {
        base.Enter();
        _controller.velocity.y = 0;
        if (_controller._charMove.downInfo.collider != null)
        {
            _controller.transform.position = _controller._charMove.downInfo.point + new Vector3(0, 0.9f, 0);
        }
        _controller.IsJumpEarlyUp = false;
        _controller.CoyoteJumpTimer = _controller.CoyoteJump;
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (_controller.CanJump)
        {
            _controller.SwitchState(_moveFactory.Jump());
            _controller.DebugLog("switch to Jump state");
        }

        if (_controller.velocity.y < 0) 
        {
            _controller.DebugLog("ÏÂÂä");
            _controller.SwitchState(_moveFactory.Fall());
        }
    }
    public override void Exit()
    {
        
    }
}
