using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    BaseState currentState;

    private void Start()
    {
        currentState = GetInitialstate();
        if (currentState != null)
        {
            currentState.Enter();
        }
    }

    private void Update()
    {
        if (currentState != null)
        {
            currentState.UpdateLogic();
        }
    }

    private void LateUpdate()
    {
        if (currentState != null)
        {
            currentState.UpdatePhysics();
        }
    }

    public void ChangeState(BaseState newState)
    {
        currentState.Exit();

        currentState = newState;
        currentState.Enter();
    }

    protected virtual BaseState GetInitialstate()
    {
        return null;
    }

    private void OnGUI()
    {
        string content = currentState != null ? currentState.name : "no current state";
        GUILayout.Label($"<color='black'><size=40>{content}</size></color>");
    }
}