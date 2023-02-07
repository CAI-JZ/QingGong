using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : BaseState
{
    private MovementSM _movementSM;
    private float horizontalInput;
    private CharacterData characterData;

    public Move(MovementSM stateMachine, CharacterData charData) : base("Move", stateMachine,charData) 
    { 
        _movementSM = stateMachine;
        characterData = charData;
    }

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
        characterData.MoveAcceleraton();
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
