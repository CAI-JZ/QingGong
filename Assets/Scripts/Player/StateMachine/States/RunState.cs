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
        if (_controller.InputDir.x == 0 )
        {
            _controller.SwitchState(_moveFactory.Idle());
        }
        CalculateWalk();
        SlopeMove();
    }


    public override void Exit()
    {
        
    }

    // speed acceleration when input
    private void CalculateWalk()
    {
        if (_controller.InputDir.x == 0)
        {
            return;
        }
        float dirMul = _controller.InputDir.x > 0 ? 1 : -1;

        //speed acceleration when input
        _controller.currentVelX += _controller.moveAcceleration* dirMul * Time.deltaTime;
        _controller.currentVelX = Mathf.Clamp(_controller.currentVelX, _controller.MaxSpeed *-1f, _controller.MaxSpeed);
    }

    private void SlopeMove()
    {
        if(_controller.CanSlopeWalk)
        {
            _controller.currentVelX = _controller.WallForward.x * _controller.InputDir.x * 10;
            _controller.velocity.y = _controller.WallForward.y * 10;
        }
    }

}
