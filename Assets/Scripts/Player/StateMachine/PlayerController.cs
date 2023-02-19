using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : StateMachine
{
    [Header("Reference")]
    private MovementStateFactory _moveFactory;
    public CharacterMovement _charMove;

    [Header("Base Data")]
    [SerializeReference] private float maxSpeed;
    [SerializeField]public Vector3 velocity; 

    [Header("Move")]
    public float moveAcceleration = 50f;
    public float deAcceleration = 50f;
    [SerializeField]private float inputDir;
    public float currentVelocityX;

    [Header("Jump")]
    [SerializeField] private float jumpHight = 30f;
    [SerializeField] private float jumpApexThreshold = 10f;
    [SerializeField] private float jumpEarlyMul = 3f;
    [SerializeField] private float coyoteJump = 0.2f;
    [SerializeField] private float jumpInputBuffer = 0.2f;
    private float coyoteJumpTimer;
    private float jumpInputBufferTimer;
    private bool jumpInputDown;
    private bool jumpInputUp;
    [SerializeField] private float apexPoint;

    [Header("GRAVITY")]
    [SerializeField] private float gravityClamp = -30f;
    [SerializeField] private float minFallGravity = 80f;
    [SerializeField] private float maxFallGraviyt = 120f;
    private bool isJumpEarlyUp;
    [SerializeField] private float fallGravity;
    [SerializeField]private float fallSpeed;

    public bool isControl = true;
    //[SerializeField]private bool downRay;
    //[SerializeField]private float rayDisDown;

    public float InputDir => inputDir;
    public float MaxSpeed => maxSpeed;
    public float FallSpeed => fallSpeed;
    public float GravityClamp => gravityClamp;
    //public bool JumpInputDown => jumpInputDown;
    public bool JumpInputUp => jumpInputUp;
    public float JumpHight => jumpHight;
    public float CoyoteJump => coyoteJump;
    public float JumpInputBuffer => jumpInputBuffer;

    public bool IsJumpEarlyUp { get { return isJumpEarlyUp; } set { isJumpEarlyUp = value; } }
    public float CoyoteJumpTimer { get { return coyoteJumpTimer; } set { coyoteJumpTimer = value; } }
    public float JumpInputBufferTimer { get { return jumpInputBufferTimer; } set { jumpInputBufferTimer = value; } }
    public bool IsTouchWall => (velocity.x > 0 && _charMove.rightRay) || (velocity.x < 0 && _charMove.leftRay);
    public bool IsGrounded => _charMove.downRay; //&& _charMove.downInfo.collider.tag == "Ground";
    public bool CanJump => jumpInputBufferTimer > 0 && coyoteJumpTimer > 0;
    public bool CheckIsJumpEarly => !_charMove.downRay && jumpInputUp && velocity.y > 0;

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
        InputDetector();
        //downRay = Physics.Raycast(transform.position, Vector3.down, rayDisDown, (1 << 10 | 1 << 6));
        CalculateJumpApex();
        base.Update();
        CalculateGravity();
        if (!IsGrounded)
        {
            velocity.y -= fallSpeed * Time.deltaTime;
            if (velocity.y < gravityClamp) velocity.y = gravityClamp;
        }
        CharacterMove();
        CheckWall();   
    }

    private void CharacterMove()
    {
        velocity.x = currentVelocityX * inputDir;
        transform.position += velocity * Time.deltaTime;
    }

    private void CheckWall()
    {
        if (IsTouchWall)
        {
            velocity.x = 0;
            SwitchState(_moveFactory.Idle());
        }
    }

    private void CalculateJumpApex()
    {
        if (!_charMove.downRay)
        {
            apexPoint = Mathf.InverseLerp(jumpApexThreshold, 0, Mathf.Abs(velocity.y));
            fallGravity = Mathf.Lerp(minFallGravity, maxFallGraviyt, apexPoint);
        }
        else
        {
            apexPoint = 0;
        }

    }

    private void CalculateGravity()
    {
        if (CheckIsJumpEarly)
        {
            fallSpeed = fallGravity * jumpEarlyMul;
            Debug.Log("IsJumpEarly");
        }
        else
        {
            fallSpeed = fallGravity;
        }
        if (velocity.y < 0 && IsGrounded)
        {
            velocity.y = 0;
            return;
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
