using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : BaseState
{
    private MovementSM _movementSM;
    private float horizontalInput;
    private CharacterData characterData;

    public Idle (MovementSM stateMachine,CharacterData charData) : base("Idle", stateMachine,charData)
    {
        _movementSM = stateMachine;
        characterData = charData;
    }

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
        characterData.MoveDeacceleration();
        if (Mathf.Abs(horizontalInput) > Mathf.Epsilon)
        {
            stateMachine.ChangeState(_movementSM.moveState);
        }
    }
}
