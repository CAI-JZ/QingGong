using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : StateMachine
{
    [Header("Reference")]
    private MovementStateFactory _moveFactory;
    private CharacterMovement _charMove;

    [Header("Base Data")]
    [SerializeReference] private float maxSpeed;
    [SerializeField]public Vector3 velocity; 

    [Header("Move")]
    public float moveAcceleration = 50f;
    public float deAcceleration = 50f;
    [SerializeField]private float inputDir;

    [Header("Jump")]
    private bool jumpInputDown;
    private bool jumpInputUp;

    public bool isControl = true;

    public float InputDir => inputDir;
    public float MaxSpeed => maxSpeed;

    public bool IsTouchWall => (velocity.x > 0 && _charMove.rightRay) || (velocity.x < 0 && _charMove.leftRay);
    public bool IsGrounded => _charMove.downRay && _charMove.downInfo.collider.tag == "Ground";

    private void Awake()
    {
        _charMove = GetComponent<CharacterMovement>();
        _moveFactory = new MovementStateFactory(this);
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {

        base.Update();
        InputDetector();
        CheckWall();
    }

    private void CheckWall()
    {
        if (IsTouchWall)
        {
            velocity.x = 0;
            SwitchState(_moveFactory.Idle());
        }
    }

    protected override BaseState GetInitialState()
    {
        if (_moveFactory == null)
        {
            Debug.LogError("moveFactory is null");
            return null;
        }
        else
        {   
            return _moveFactory.Idle();
        }
    }

    private void InputDetector()
    {
        inputDir = PlayerInput._instance.moveDir;
        jumpInputDown = PlayerInput._instance.jumpBtnDown;
        jumpInputUp = PlayerInput._instance.jumpBtnUp;
    }
}
