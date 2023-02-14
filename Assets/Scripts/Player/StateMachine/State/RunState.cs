public class RunState : BaseState
{
    public RunState(StateMachine currentContext, StateFactory stateFactory) : base(currentContext, stateFactory,"Run") { }

    public override void Enter()
    {
        
    }

    public override void UpdateState()
    {
        
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
