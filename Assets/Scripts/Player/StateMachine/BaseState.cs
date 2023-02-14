public abstract class BaseState
{
    protected StateMachine _ctx;
    protected StateFactory _factory;
    protected BaseState _currentSubstate;
    protected BaseState _currentSuperstate;
    public string stateName;

    public BaseState(StateMachine currentContext, StateFactory stateFactory, string name)
    {
        _ctx = currentContext;
        _factory = stateFactory;
        stateName = name;
    }

    public abstract void Enter();
    public abstract void UpdateState();
    public abstract void Exit();
    public abstract void CheckSwitchState();
    public abstract void InitializeSubState();

    public void UpdateStates() 
    {
        UpdateState();
        if (_currentSubstate != null)
        {
            _currentSubstate.UpdateStates();
        }
    }
    protected void SwitchState(BaseState newState) 
    {
        //current state exits state
        Exit();
        //enter  new state
        newState.Enter();
        // update current state to new staste
        _ctx.CurrentState = newState;
    }
    protected void SetSuperState(BaseState newSuperState) 
    {
        _currentSuperstate = newSuperState;
       
    }

    protected void SetSubState(BaseState newSubState)
    {
        _currentSubstate = newSubState;
        newSubState.SetSuperState(this);
    }
}
