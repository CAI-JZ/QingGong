using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : BaseState
{
    private MovementSM _movementSM;
    private float horizontalInput;

    public Move(MovementSM stateMachine) : base("Move", stateMachine) { _movementSM = stateMachine; }

    public override void Enter()
    {
        base.Enter();
        horizontalInput = 0; 
        _movementSM.meshRender.material.color = Color.red;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        horizontalInput = PlayerInput._instance.moveDir;
        if (Mathf.Abs(horizontalInput) < Mathf.Epsilon)
        {
            stateMachine.ChangeState(_movementSM.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("Stop");
    }
}
