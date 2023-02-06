using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSM : StateMachine
{
    public Idle idleState;
    public Move moveState;
    public MeshRenderer meshRender;

    private void Awake()
    {
        idleState = new Idle(this);
        moveState = new Move(this);
        meshRender = transform.GetChild(0).GetComponentInChildren<MeshRenderer>();
    }

    protected override BaseState GetInitialstate()
    {
        return idleState;
    }
}
