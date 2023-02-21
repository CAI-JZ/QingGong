using UnityEngine;

public class JumpState : MovementBaseState
{
    public JumpState(StateMachine stateMachine, StateFactory factory) : base(stateMachine, factory, "Jump") {}

    public override void Enter()
    {
        base.Enter();
        HandleJump();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (_controller.velocity.y < 0)
        {
            _controller.SwitchState(_moveFactory.Fall());
        }
    }


    public override void Exit()
    {
        base.Enter();
        //when exit
        //stop animator;
    }

    private void HandleJump()
    {
         _controller.velocity.y = _controller.JumpHight;
         _controller.JumpInputBufferTimer = 0;
        
        if (_controller.CheckIsJumpEarly)
        {
            _controller.IsJumpEarlyUp = true;
        }
    }


}
