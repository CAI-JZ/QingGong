using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseState
{
    public string name;
    protected StateMachine stateMachine;
    protected CharacterData charData;

    public BaseState(string name, StateMachine stateMachine, CharacterData charData)
    {
        this.name = name;
        this.stateMachine = stateMachine;
        this.charData = charData;
    }

    public virtual void Enter() { }
    public virtual void UpdateLogic() { }
    public virtual void UpdatePhysics() { }
    public virtual void Exit() { }

}
