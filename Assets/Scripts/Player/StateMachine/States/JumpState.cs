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
        base.Enter();
        _controller.JumpInputBufferTimer = _controller.JumpInputBuffer;
        HandleJump();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (_controller.velocity.y < 0)
        {
            _controller.SwitchState(_moveFactory.Fall());
        }
        //if (_controller.IsGrounded)
        //{
        //    BaseState newstate = _controller.velocity.x != 0 ? _moveFactory.Idle() : _moveFactory.Run();
        //    _controller.SwitchState(newstate);
        //}
        JumpOptimazation();
    }


    public override void Exit()
    {
        base.Enter();
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
        if (_controller.CheckIsJumpEarly)
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
