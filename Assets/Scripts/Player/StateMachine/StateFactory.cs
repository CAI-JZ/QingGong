public class StateFactory
{
    StateMachine _context;

    public StateFactory(StateMachine currentContext)
    {
        _context = currentContext;
    }

    public BaseState Idle() { return new IdleState(_context,this); }
    public BaseState Run() { return new RunState(_context, this); }
    public BaseState Jump() { return new JumpState(_context, this); }
    public BaseState Grounded() { return new GroundedState(_context, this); }
    
}
