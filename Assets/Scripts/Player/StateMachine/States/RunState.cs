using UnityEngine;

public class RunState : GroundedState
{
    public RunState(StateMachine stateMachine, StateFactory factory) : base(stateMachine, factory, "Run") { }

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
        CalculateWalk();
    }


    public override void Exit()
    {
        
    }

    // speed acceleration when input
    private void CalculateWalk()
    {
        // speed acceleration when input
        _controller.currentVelocityX += _controller.moveAcceleration * Time.deltaTime;
        _controller.currentVelocityX = Mathf.Clamp(_controller.currentVelocityX, 0, _controller.MaxSpeed);
    }

}
