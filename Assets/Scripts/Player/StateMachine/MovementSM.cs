using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSM : StateMachine
{
    public Idle idleState;
    public Move moveState;
    public MeshRenderer meshRender;
    private CharacterData charData;

    private void Awake()
    {
        charData = GetComponent<CharacterData>();
        idleState = new Idle(this, charData);
        moveState = new Move(this,charData);
        meshRender = transform.GetChild(0).GetComponentInChildren<MeshRenderer>();
    }

    protected override BaseState GetInitialstate()
    {
        return idleState;
    }
}
