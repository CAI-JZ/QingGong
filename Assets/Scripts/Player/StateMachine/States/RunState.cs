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
        if (_controller.InputDir.x == 0 )
        {
            _controller.SwitchState(_moveFactory.Idle());
        }
        CalculateWalk();
        SlopeWalk();
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

    private void SlopeWalk()
    {
        if(_controller.CanSlopeWalk)
        {
            //return velocity vector3
            _controller.currentVelX = _controller.WallForward.x * _controller.InputDir.x * 10;
            _controller.currentVelY = _controller.WallForward.y * _controller.InputDir.x * 10;
           
        }
    }

}
