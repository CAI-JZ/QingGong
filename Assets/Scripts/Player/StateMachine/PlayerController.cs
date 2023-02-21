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
    [SerializeField]private Vector2 inputDir;
    public float currentVelX;
    public float currentVelY;

    [Header("Jump")]
    [SerializeField] private float jumpHight = 30f;
    [SerializeField] private float jumpApexThreshold = 10f;
    [SerializeField] private float jumpEarlyMul = 3f;
    [SerializeField] private float coyoteJump = 0.2f;
    [SerializeField] private float jumpInputBuffer = 0.2f;
    [SerializeField] private float fallHorizontalMul = 10;
    private float coyoteJumpTimer;
    private float jumpInputBufferTimer;
    private bool jumpInputDown;
    private bool jumpInputUp;
    [SerializeField] private float apexPoint;

    [Header("Dash")]
    [SerializeField] private float dashPower;
    [SerializeField] private float deshAcceleration;
    [SerializeField] private float dashDeceleration;

    [Header("GRAVITY")]
    [SerializeField] private float gravityClamp = -30f;
    [SerializeField] private float minFallGravity = 80f;
    [SerializeField] private float maxFallGraviyt = 120f;
    private bool isJumpEarlyUp;
    [SerializeField] private float fallGravity;
    [SerializeField]private float fallSpeed;
    [SerializeField]private bool useGravity;

    private bool isControl ;
    [SerializeField]private bool isground;

    public Vector2 InputDir => inputDir;
    public float MaxSpeed => maxSpeed;
    public float JumpHight => jumpHight;
    public float CoyoteJump => coyoteJump;
    public float FallHorizontalMul => fallHorizontalMul;
    public float DashPower => dashPower;
    public float DashAcceleration => deshAcceleration;
    public float DashDeceleration => dashDeceleration;
    public bool UseGravity { get { return useGravity; } set { useGravity = value; } }
    public bool IsControllable { get { return isControl; } set { isControl = value; } }
    public bool IsJumpEarlyUp { get { return isJumpEarlyUp; } set { isJumpEarlyUp = value; } }
    public float CoyoteJumpTimer { get { return coyoteJumpTimer; } set { coyoteJumpTimer = value; } }
    public float JumpInputBufferTimer { get { return jumpInputBufferTimer; } set { jumpInputBufferTimer = value; } }
    public bool IsTouchWall => (velocity.x > 0 && _charMove.rightRay) || (velocity.x < 0 && _charMove.leftRay);
    public bool IsGrounded => _charMove.downRay; //&& _charMove.downInfo.collider.tag == "Ground";
    public bool CanJump => jumpInputBufferTimer > 0 && coyoteJumpTimer > 0;
    public bool CheckIsJumpEarly => !_charMove.downRay && jumpInputUp && velocity.y > 0;
    public bool CanSlopeWalk => _charMove.canSlopeWalkOn;

    private void Awake()
    {
        _charMove = GetComponent<CharacterMovement>();
        _moveFactory = new MovementStateFactory(this);
        isControl = true;
        useGravity = true;
    }

    protected override void Start()
    {
        base.Start();
        Vector2 a = Vector2.left;
        Debug.Log("Left" + a);
        a = Vector2.right;
        Debug.Log("right" + a);
        a = Vector2.up;
        Debug.Log("up" + a);
        a = Vector2.down;
        Debug.Log("down" + a);
        a = Vector2.up + Vector2.right;
        Debug.Log("up+right" + a);

    }

    protected override void Update()
    {
        InputDetector();
        CalculateJumpApex();
        base.Update();
        CalculateGravity();
        isground = IsGrounded;
        JumpOptimazation();
        CharacterMove();
        CheckWall();   
    }

    private void CharacterMove()
    {
        if (!useGravity)
        {
            velocity.y = 0;
        }
        else if (!IsGrounded && useGravity)
        {
            velocity.y -= fallSpeed * Time.deltaTime;
            if (velocity.y < gravityClamp) velocity.y = gravityClamp;
        }
        if (isControl)
        {
            float dir = inputDir.x > 0 ? 1 : -1;
            velocity.x = currentVelX;
            transform.position += velocity * Time.deltaTime;
        } 
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

    private void JumpOptimazation()
    {
        if (!IsGrounded)
        {
            coyoteJumpTimer -= Time.deltaTime;
            coyoteJumpTimer = Mathf.Clamp(coyoteJumpTimer, -0.2f, coyoteJump);
        }
        //Input Buffer
        if (jumpInputDown)
        {
            jumpInputBufferTimer = jumpInputBuffer;
        }
        else
        {
            jumpInputBufferTimer -= Time.deltaTime;
            if (jumpInputBufferTimer < 0) jumpInputBufferTimer = 0;
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
        inputDir.x = PlayerInput._instance.HorizontalInput;
        inputDir.y = PlayerInput._instance.VerticalInput;
        jumpInputDown = PlayerInput._instance.jumpBtnDown;
        jumpInputUp = PlayerInput._instance.jumpBtnUp;
    }
}
