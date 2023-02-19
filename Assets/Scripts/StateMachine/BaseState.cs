public abstract class BaseState
{
    protected StateMachine _stateMachine;
    protected StateFactory _factory;
    public string stateName;

    public BaseState(StateMachine stateMachine, StateFactory factory, string name)
    {
        _stateMachine = stateMachine;
        _factory = factory;
        stateName = name;
    }

    public virtual void Enter() { }
    public virtual void UpdateState() { }
    public virtual void Exit() { }
}
