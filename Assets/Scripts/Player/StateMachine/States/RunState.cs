using UnityEngine;

public class RunState : GroundedState
{
    public RunState(PlayerController stateMachine, MovementStateFactory factory) : base(stateMachine, factory, "Run") { }

    public override void Enter()
    {
        base.Enter();
        _controller.DebugLog("Run");
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (_controller.InputDir == 0 )
        {
            _controller.SwitchState(_moveFactory.Idle());
        }
        
    }

    public override void UpdatePhysic()
    {
        base.UpdatePhysic();
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
