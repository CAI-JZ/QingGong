using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [Header("Test for STATEMACHINE")]
    //test for state machine
    Rigidbody rb;
    [SerializeField] float jumpPower = 5;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask gourndLayer;
    public float moveDir;

    [SerializeField]private bool isGrounded;
    private bool isJumpPressed;

    [SerializeField]BaseState _currentState;
    StateFactory _states;



    public BaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public bool IsJumpPressed { get { return isJumpPressed; } }
    public bool IsGrounded { get { return isGrounded; } }

    private void Awake()
    {
        _states = new StateFactory(this);
        _currentState = _states.Grounded();
        _currentState.Enter();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        CheckPlayerInput();
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.1f, gourndLayer);
        _currentState.UpdateState();
        

       
        
    }

    private void OnGUI()
    {
        string content = _currentState != null ? _currentState.stateName : "no current state";
        GUILayout.Label($"<color='black'><size=40>{content}</size></color>");
    }

    private void CheckPlayerInput()
    {
        isJumpPressed = PlayerInput._instance.jumpBtnDown;
        moveDir = PlayerInput._instance.moveDir;
    }

    public void JumpTest()
    {
        Debug.Log("我跳了，你随意");
        rb.velocity = new Vector3(rb.velocity.x, jumpPower, rb.velocity.z);
    }

    public void GroundExit()
    {
        Debug.Log("我要闹了！");
    }

    public void DebugLog(string text)
    {
        Debug.Log(text);
    }
}