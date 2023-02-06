using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : BaseState
{
    private MovementSM _movementSM;
    private float horizontalInput;

    public Idle (MovementSM stateMachine) : base("Idle", stateMachine) { _movementSM = stateMachine; }

    public override void Enter()
    {
        base.Enter();
        horizontalInput = 0;
        _movementSM.meshRender.material.color= Color.black;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        horizontalInput = PlayerInput._instance.moveDir;
        if (Mathf.Abs(horizontalInput) > Mathf.Epsilon)
        {
            stateMachine.ChangeState(_movementSM.moveState);
        }
    }
}
