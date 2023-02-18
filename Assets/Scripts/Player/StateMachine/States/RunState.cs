using UnityEngine;

public class RunState : GroundedState
{
    public RunState(PlayerController stateMachine, MovementStateFactory factory) : base(stateMachine, factory, "Run") { }

    public override void Enter()
    {
        _controller.DebugLog("进入到跑步状态了");
    }

    public override void UpdateState()
    {
        if (_controller.InputDir == 0)
        {
            if (_controller.IsTouchWall)
            {
                _controller.velocity.x = 0;
            }
            _controller.SwitchState(_moveFactory.Idle());
        }
        CalculateWalk();
    }

    public override void Exit()
    {
        
    }

    // speed acceleration when input
    private void CalculateWalk()
    {
        // speed acceleration when input
        _controller.velocity.x += _controller.InputDir * _controller.moveAcceleration * Time.deltaTime;
        _controller.velocity.x = Mathf.Clamp(_controller.velocity.x, -_controller.MaxSpeed, _controller.MaxSpeed);
    }

}
