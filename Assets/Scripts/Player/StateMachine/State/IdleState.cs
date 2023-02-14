public class IdleState : BaseState
{
    public IdleState(StateMachine currentContext, StateFactory stateFactory) : base(currentContext, stateFactory,"Idle") { }

    public override void Enter()
    {
        //_ctx.DebugLog("Idle");
        _ctx.render.color = UnityEngine.Color.black;
    }

    public override void UpdateState()
    {
        CheckSwitchState();
        _ctx.render.color = UnityEngine.Color.black;
    }

    public override void Exit()
    {
        
    }

    public override void InitializeSubState()
    {
        
    }

    public override void CheckSwitchState()
    {
        if (_ctx.moveDir != 0)
        {
            SwitchState(_factory.Run());
        }
    }
}
