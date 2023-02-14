using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedState : BaseState
{
    public GroundedState(StateMachine currentContext, StateFactory stateFactory) : base(currentContext, stateFactory,"Grounded")
    {
        InitializeSubState();
    }

    public override void Enter()
    {
        _ctx.DebugLog("进入到落地状态了");
    }

    public override void UpdateState()
    {
        CheckSwitchState();
        InitializeSubState();
    }

    public override void Exit()
    {
        _ctx.GroundExit();
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
        if (_ctx.IsJumpPressed)
        {
            SwitchState(_factory.Jump());
        }
    }
}
