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
    public Vector3 velocity; 

    [Header("Move")]
    public float moveAcceleration = 50f;
    public float deAcceleration = 50f;
    [SerializeField]private float inputDir;

    [Header("Jump")]
    private bool jumpInputDown;
    private bool jumpInputUp;

    public float InputDir => inputDir;
    public float MaxSpeed => maxSpeed;

    public bool IsTouchWall => velocity.x > 0 && _charMove.rightRay || velocity.x < 0 && _charMove.leftRay;

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
        if (IsTouchWall)
        {
            Debug.Log(IsTouchWall);
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
            Debug.LogWarning("Ë³Àû»ñÈ¡Idle");
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
