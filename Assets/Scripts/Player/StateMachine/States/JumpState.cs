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
        if (_controller.currentVelY < 0)
        {
            _controller.SwitchState(_moveFactory.Fall());
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _controller.SwitchState(_moveFactory.Dash());
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
        Debug.Log("Handle Jump");
         _controller.currentVelY = _controller.JumpHight;
         _controller.JumpInputBufferTimer = 0;
        
        if (_controller.CheckIsJumpEarly)
        {
            _controller.IsJumpEarlyUp = true;
        }
    }


}
