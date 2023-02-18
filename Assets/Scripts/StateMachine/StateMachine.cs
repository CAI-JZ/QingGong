using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    protected StateFactory _factory;

    protected virtual BaseState CurrentState
    {
        get => _currentState;
        set => Transition(value);
    }

    protected BaseState _currentState;
    protected bool _inTransition;

    protected virtual void Start()
    {
        _currentState = GetInitialState();
        if (_currentState != null)
        {
            _currentState.Enter();
        }
    }

    protected virtual void Update()
    {
        if (_currentState != null)
        {
            _currentState.UpdateState();
        }
    }


    public virtual void Transition(BaseState value)
    {
        // Check if can switch
        if (_currentState == value || _inTransition)
        {
            return;
        }

        // Exit cuurent state
        _inTransition = true;

        if (_currentState != null)
        {
            _currentState.Exit();
        }

        // Enter new state;
        _currentState = value;

        if (_currentState != null)
        {
            _currentState.Enter();
        }

        _inTransition = false;
    }

    public virtual void SwitchState(BaseState newState) 
    {
        CurrentState = newState;
    }

    protected virtual BaseState GetInitialState()
    {
        return null;
    }

    protected void OnGUI()
    {
        string content = _currentState != null ? _currentState.stateName : "Null";
        GUILayout.Label($"<color='black'><size=40>{content}</size></color>");
    }

    public void DebugLog(string log)
    {
        Debug.Log(log);
    }
}