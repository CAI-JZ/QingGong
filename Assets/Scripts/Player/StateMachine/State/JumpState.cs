public class JumpState : BaseState
{
    public JumpState(StateMachine currentContext, StateFactory stateFactory) : base(currentContext, stateFactory,"Jump") 
    {
        isRootState = true;
        InitializeSubState();
    }


    public override void Enter()
    {
        
        HandleJump();
    }

    public override void UpdateState()
    {
        CheckSwitchState();
    }

    public override void Exit()
    {
        //when exit
        //stop animator;
        //cool down jump
        _ctx.DebugLog("�뿪��Ծ״̬");
    }

    public override void InitializeSubState()
    {
        if (_ctx.moveDir == 0)
        {
            SetSubState(_factory.Idle());
        }
        else
        {
            SetSubState(_factory.Run());
        }
    }

    public override void CheckSwitchState()
    {
        if (_ctx.IsGrounded)
        {
            _ctx.DebugLog("�����");
            SwitchState(_factory.Grounded());
        }
        else
        {
            _ctx.DebugLog("��Ҫ����");
        }
    }

    void HandleJump()
    {
        _ctx.JumpTest();
    }

    void ExitJump()
    {
        
    }
}
