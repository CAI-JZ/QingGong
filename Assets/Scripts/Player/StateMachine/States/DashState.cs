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
        _controller.UseGravity = false;
        _controller.currentVelY = 0;
        //_controller.velocity = Vector3.zero;
        CalculateDashDir();
        isDashing = true;
        accelerating = true;
}

    public override void UpdateState()
    {
        base.UpdateState();
        Debug.Log("IsDashing: " + isDashing);
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
        _controller.UseGravity = true;

    }

    private void CalculateDashDir()
    {
        if (_controller.InputDir != Vector2.zero)
        {
            dashDir = _controller.InputDir;
        }
        else
        {
            dashDir = _controller.playerDir == PlayerDir.Right ? Vector2.left : Vector2.right;
        }
        
    }

    private void HandleDash()
    {
        if (Mathf.Abs(_controller.currentVelX) >= _controller.DashPower || Mathf.Abs(_controller.currentVelY) >= _controller.DashPower)
        {
            accelerating = false;
        }
        if (accelerating)
        {
            _controller.currentVelX += dashDir.x * _controller.DashAcceleration * Time.deltaTime;
            _controller.currentVelY += dashDir.y * _controller.DashAcceleration * Time.deltaTime;
        }
        else
        {
            _controller.currentVelX = Mathf.MoveTowards(_controller.currentVelX, 0, _controller.DashDeceleration * Time.deltaTime);
            _controller.currentVelY = Mathf.MoveTowards(_controller.currentVelY, 0, _controller.DashDeceleration * Time.deltaTime);
            if (Mathf.Abs(_controller.currentVelX) < _controller.DashPower /2 +3f)
            { 
                _controller.UseGravity = true;
                isDashing = false;
            }
        }

    }
}
