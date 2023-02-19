using UnityEngine;

public class JumpState : BaseState
{
    private PlayerController _controller;
    private MovementStateFactory _moveFactory;

    public JumpState(StateMachine stateMachine, StateFactory factory) : base(stateMachine, factory, "Jump")
    {
        _controller = (PlayerController)stateMachine;
        _moveFactory = (MovementStateFactory)factory;
    }


    public override void Enter()
    {
        _controller.JumpInputBufferTimer = _controller.JumpInputBuffer;

    }

    public override void UpdateState()
    {
        if (_controller.JumpInputUp)
        {
            _controller.SwitchState(_moveFactory.Fall());
        }
        JumpOptimazation();
        
    }

    public override void UpdatePhysic()
    {
        base.UpdatePhysic();
        HandleJump();
    }

    public override void Exit()
    {
        //when exit
        //stop animator;
        //cool down jump

    }

    private void HandleJump()
    {
        if (_controller.CanJump)
        {
            _controller.velocity.y = _controller.JumpHight;
            _controller.JumpInputBufferTimer = 0;
        }
        if(_controller.CheckIsJumpEarly)
        {
            _controller.IsJumpEarlyUp = true;
        }
    }

    private void JumpOptimazation()
    {
        _controller.CoyoteJumpTimer -= Time.deltaTime;
        _controller.CoyoteJumpTimer = Mathf.Clamp(_controller.CoyoteJumpTimer, -0.2f, _controller.CoyoteJump);

        //Input Buffer
        if (!PlayerInput._instance.jumpBtnDown)
        {
            _controller.JumpInputBufferTimer -= Time.deltaTime;
            if (_controller.JumpInputBufferTimer < 0) _controller.JumpInputBufferTimer = 0;
        }
    }
}
