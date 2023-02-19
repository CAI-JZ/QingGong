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
        _controller.velocity.y = 0;
        _controller.transform.position = _controller._charMove.downInfo.point + new Vector3(0, 0.9f, 0);
        _controller.CoyoteJumpTimer = _controller.CoyoteJump;
        _controller.IsJumpEarlyUp = false;
    }

    public override void UpdateState()
    {
        if (PlayerInput._instance.jumpBtnDown)
        {
            _controller.SwitchState(_moveFactory.Jump());
        }
        if (!_controller.IsGrounded)
        {
            _controller.DebugLog("ÏÂÂä");
            _controller.SwitchState(_moveFactory.Fall());
        }
    }

    public override void UpdatePhysic()
    {
        base.UpdatePhysic();
    }

    public override void Exit()
    {
        
    }
}
