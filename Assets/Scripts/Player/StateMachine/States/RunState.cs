using UnityEngine;

public class RunState : GroundedState
{
    public RunState(StateMachine stateMachine, StateFactory factory) : base(stateMachine, factory, "Run") { }

    public override void Enter()
    {
        base.Enter();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (_controller.InputDir.x == 0 || _controller.RLTouched && !_controller.CanSlopeWalk)
        {
            _controller.SwitchState(_moveFactory.Idle());
        }
        if (_controller.CanWallRun)
        {
            _controller.SwitchState(_moveFactory.WallRun());
        }
        CalculateWalk();
        //SlopeWalk();
    }


    public override void Exit()
    {
        
    }

    // speed acceleration when input
    private void CalculateWalk()
    {
        float dirMul = _controller.InputDir.x > 0 ? 1 : -1;
        
        Vector2 runDir = _controller.CanSlopeWalk ? dirMul * (Vector2)_controller.WallForward : dirMul * Vector2.right;

        _controller.currentVelX += runDir.x * _controller.moveAcceleration * Time.deltaTime;
        //_controller.currentVelY += runDir.y * _controller.moveAcceleration * Time.deltaTime;
        _controller.currentVelX = Mathf.Clamp(_controller.currentVelX, _controller.MaxSpeed * -1f, _controller.MaxSpeed);
        //_controller.currentVelY = Mathf.Clamp(_controller.currentVelY, _controller.MaxSpeed * -1f, _controller.MaxSpeed);
 
    }

    

}
