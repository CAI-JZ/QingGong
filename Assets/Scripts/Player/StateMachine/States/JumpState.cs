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
        //_ctx.rb.mass = 2;

    }

    public override void UpdateState()
    {

    }

    public override void Exit()
    {
        //when exit
        //stop animator;
        //cool down jump

    }

    //public override void CheckSwitchState()
    //{
    //    if (_ctx.IsGrounded)
    //    {
    //        _ctx.DebugLog("在落地");
    //        SwitchState(_factory.Grounded());
    //    }
    //    //else if (_ctx.rb.velocity.y < 0)
    //    //{
    //    //    _ctx.DebugLog("正在下落");
    //    //    SwitchState(_factory.Fall());

    //    //}


}
