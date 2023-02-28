using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerDir
{ 
    Right,
    Left,
}

public class PlayerController : StateMachine
{
    [Header("Reference")]
    private MovementStateFactory _moveFactory;
    public CharacterMovement _charMove;

    [Header("Base Data")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private Vector2 inputDir;
    

    public PlayerDir playerDir = PlayerDir.Right;

    [Header("Move")]
    public float moveAcceleration = 50f;
    public float deAcceleration = 50f;
    public float currentSpeed; //速度，用于实际控制当前的速度
    public Vector3 currentVelocity; //用于实际控制当前的移动方向
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
    private float apexPoint;

    [Header("Slope")]
    [SerializeField]private float slopeCheckDis;
    private Vector2 slopeNormalPerp;
    private float slopeDownAngle;
    private float slopeDownAngleOld;
    private bool isOnSlope;

    [Header("Wall State")]
    [SerializeField] private Vector2 wallJumpPower;

    [Header("Dash")]
    [SerializeField] private float dashPower;
    [SerializeField] private float deshAcceleration;
    [SerializeField] private float dashDeceleration;

    [Header("GRAVITY")]
    [SerializeField] private float gravityClamp = -30f;
    [SerializeField] private float minFallGravity = 80f;
    [SerializeField] private float maxFallGraviyt = 120f;
    [SerializeField] private float fallGravity;
    [SerializeField] private float fallSpeed;

    private bool jumpInputDown;
    private bool jumpInputUp;
    [SerializeField] private bool useGravity;
    [SerializeField] private bool inputEnable;
    [SerializeField] private bool isAlive;

    // variables;
    public Vector2 InputDir => inputDir;
    public float MaxSpeed => maxSpeed;
    public float JumpHight => jumpHight;
    public float FallHorizontalMul => fallHorizontalMul;
    public float DashPower => dashPower;
    public float DashAcceleration => deshAcceleration;
    public float DashDeceleration => dashDeceleration;
    public Collider2D GroundRef => _charMove.Grounded.collider;
    public bool UseGravity { get { return useGravity; } set { useGravity = value; } }
    public bool InputEnable { get { return inputEnable; } set { inputEnable = value; } }
    //
    public bool RLTouched => (inputDir.x > 0 && _charMove.rightHit) || (inputDir.x < 0 && _charMove.leftHit);
    public bool CanWallRun => (inputDir.x > 0 && _charMove.rightHit && _charMove.rightHit.collider.tag == "Wall") || (inputDir.x < 0 && _charMove.leftHit && _charMove.leftHit.collider.tag == "Wall");
    public RaycastHit2D WallRef => _charMove.rightHit ? _charMove.rightHit : _charMove.leftHit;
    public bool IsGrounded => _charMove.Grounded;
    public bool CanJump => jumpInputBufferTimer > 0 && coyoteJumpTimer > 0;
    public bool CheckIsJumpEarly => !_charMove.Grounded && jumpInputUp && velocity.y > 0;
    public bool CanSlopeWalk => _charMove.wallAngle > Mathf.Epsilon ? true : false;
    public Vector3 WallForward => _charMove.wallForward;

    public void ResetVelocity()
    {
        velocity = Vector3.zero;
        //currentVelY = 0;
        //currentVelX = 0;
    }

    private void Awake()
    {
        _charMove = GetComponent<CharacterMovement>();
        _moveFactory = new MovementStateFactory(this);
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Initialize Data
    /// </summary>
    protected override void Start()
    {
        isAlive = true;
        inputEnable = true;
        useGravity = true;
        base.Start();
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

    protected override void Update()
    {
        if (!isAlive)
        {
            return;
        }
        InputDetector();
        FlipPlayerDir();
        SlopeCheck();
        CalculateGravity();
        CalculateJumpApex();
        JumpOptimazation();

        base.Update();
        CharacterMove();
        Physics.SyncTransforms();
    }

    private void CharacterMove()
    {
        velocity.x = currentVelX;
        velocity.y = currentVelY;
        //velocity = currentVelocity * currentSpeed;
        transform.position += velocity * Time.deltaTime;
    }

    private void HandleMove()
    {
        if (IsGrounded && !isOnSlope)
        {

        }
        else if (IsGrounded && isOnSlope)
        {

        }
        else if (!IsGrounded)
        { 
            //currentVelX = movementSpeed * xInput
            //current
        }
    }

    /// <summary>
    /// Jump/Gravity Calculate
    /// </summary>
    private void CalculateJumpApex()
    {
        if (!_charMove.Grounded)
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
        if (velocity.y < 0 && IsGrounded || !useGravity)
        {
            fallSpeed = 0;
            return;
        }
        else if (!IsGrounded && useGravity)
        {
            if (CheckIsJumpEarly)
            {
                fallSpeed = fallGravity * jumpEarlyMul;
            }
            else
            {
                fallSpeed = fallGravity;
            }
        }
        currentVelY -= fallSpeed * Time.deltaTime;
        if (velocity.y < gravityClamp) velocity.y = gravityClamp;
    }

    private void JumpOptimazation()
    {
        //coyote Jump
        if (!IsGrounded)
        {
            coyoteJumpTimer -= Time.deltaTime;
            coyoteJumpTimer = Mathf.Clamp(coyoteJumpTimer, -0.2f, coyoteJump);
        }
        else
        {
            coyoteJumpTimer = coyoteJump;
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

    private void FlipPlayerDir()
    {
        if (inputDir.x > 0)
        {
            playerDir = PlayerDir.Right;
        }
        else if (inputDir.x < 0)
        {
            playerDir = PlayerDir.Left;
        }
    }

    /// <summary>
    /// Input
    /// </summary>
    private void InputDetector()
    {
        if (!inputEnable)
        {
            return;
        }
        inputDir.x = PlayerInput._instance.HorizontalInput;
        inputDir.y = PlayerInput._instance.VerticalInput;
        jumpInputDown = PlayerInput._instance.jumpBtnDown;
        jumpInputUp = PlayerInput._instance.jumpBtnUp;
    }

    /// <summary>
    /// Slope Check
    /// </summary>
    private void SlopeCheck()
    {
        SlopeCheckVertial();
    }

    private void SlopeCheckHorizontal()
    {

    }

    private void SlopeCheckVertial()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, slopeCheckDis, 1 << 10);

        if (hit)
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal);
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeDownAngle != slopeDownAngleOld)
            {
                isOnSlope = true;
            }
            slopeDownAngleOld = slopeDownAngle;

            Debug.DrawRay(hit.point, slopeNormalPerp, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.green);
        }
    }

}
