using System.Collections;
using UnityEngine;

public class DashState : MovementBaseState
{
    public DashState(StateMachine stateMachine, StateFactory stateFactory) : base(stateMachine, stateFactory, "Dash") { }

    private bool isDashing;
    private Vector2 dashDir;
    private bool accelerating;

    public override void Enter()
    {
        base.Enter();
        //_controller.IsControllable = false;
        _controller.UseGravity = false;
        _controller.velocity = Vector3.zero;

        dashDir = _controller.InputDir == Vector2.zero ? dashDir = Vector2.left : dashDir = _controller.InputDir;
        isDashing = true;
        accelerating = true;
}

    public override void UpdateState()
    {
        base.UpdateState();
        if (!isDashing)
        {
            if (_controller.IsGrounded)
            {
                _controller.SwitchState(_moveFactory.Idle());
            }
            else
            {
                _controller.SwitchState(_moveFactory.Fall());
            }
        }
        else
        {
            HandleDash();
        }
    }

    public override void Exit()
    {
        base.Exit();
        //_controller.IsControllable = true;
        
    }

    IEnumerator DashCount()
    {
        int i = 0;
        while (i < 9)
        {
            
        }
        isDashing = false;

        return null;
    }

    private void HandleDash()
    {
        if (Mathf.Abs(_controller.currentVelX) >= _controller.DashPower || Mathf.Abs(_controller.velocity.y) >= _controller.DashPower)
        {
            accelerating = false;
        }
        if (accelerating)
        {
            _controller.currentVelX += dashDir.x * _controller.DashAcceleration * Time.deltaTime;
            _controller.velocity.y += dashDir.y * _controller.DashAcceleration * Time.deltaTime;
        }
        else
        {
            _controller.currentVelX = Mathf.MoveTowards(_controller.currentVelX, 0, _controller.DashDeceleration * Time.deltaTime);
            _controller.velocity.y = Mathf.MoveTowards(_controller.velocity.y, 0, _controller.DashDeceleration * Time.deltaTime);

            if (_controller.velocity == Vector3.zero)
            {
                isDashing = false;
                _controller.UseGravity = true;
            }
        }

    }
}
