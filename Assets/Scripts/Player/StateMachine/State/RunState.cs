public class RunState : BaseState
{
    public RunState(StateMachine currentContext, StateFactory stateFactory) : base(currentContext, stateFactory,"Run") { }

    public override void Enter()
    {
        
    }

    public override void UpdateState()
    {
        CheckSwitchState();
        _ctx.render.color = UnityEngine.Color.green;
    }

    public override void Exit()
    {
        
    }

    public override void InitializeSubState()
    {
       
    }

    public override void CheckSwitchState()
    {
        if (_ctx.moveDir == 0)
        {
            SwitchState(_factory.Idle());
        }
    }
}
