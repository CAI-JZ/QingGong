using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharDir
{
    Right,
    Left,
}

public class MovementController : MonoBehaviour
{
    //Move
    [Header("BASIC DATA")]
    [SerializeField] Vector2 inputDir;
    [SerializeField] Vector2 velocity;
    private bool isControllable;
    public CharDir playerDir = CharDir.Right;
    [SerializeField] private float currentSpeedX;
    [SerializeField] private float currentSpeedY;

    [Header("JUMP")]
    //Character Basic data
    [SerializeField] private float jumpHight = 30f;
    [SerializeField] private float jumpApexThreshold = 10f;
    [SerializeField] private float jumpEarlyMul = 3f;
    [SerializeField] private float coyoteJump = 0.2f;
    [SerializeField] private float jumpInputBuffer = 0.2f;
    [SerializeField] private float coyoteJumpTimer;
    [SerializeField] private float jumpInputBufferTimer;
    [SerializeField] private bool jumpInputDown;
    private bool jumpInputUp;
    private float apexPoint;
    private bool isJumping;

    [Header("Wall Jump & Wall Slide")]
    [SerializeField] private bool isWallSliding;
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private float wallJumpTime = 0.2f;
    private bool isWallJumping;
    private float wallJumpDir;
    
    [SerializeField]private float wallJumpTimer;
    [SerializeField] private float wallJumpDuration = 0.4f;
    [SerializeField] private Vector2 wallJumpPower = new Vector2(8f, 16f);

    [Header("GRAVITY")]
    [SerializeField] private float gravityClamp = -30f;
    [SerializeField] private float minFallGravity = 80f;
    [SerializeField] private float maxFallGraviyt = 120f;
    private bool isJumpEarlyUp;
    [SerializeField] public float fallGravity;

    //Moves
    [Header("MOVE")]
    [SerializeField]public float moveAcceleration = 50f;
    [SerializeField]public float deAcceleration = 50f;
    [SerializeField]public float moveClamp = 13f;


    [Header("SlopeMove")]
    [SerializeField] private float slopeCheckDis;
    [SerializeField] private float maxSlopeAngle;
    private Vector2 slopeNormalPerp;
    private float slopeDownAngle;
    private float slopeDownAngleOld;
    private float slopeSideAngle;
    private bool isOnSlope;
    private bool canMoveOn;

    [Header("RAY")]
    //RayCast
    [SerializeField] private float rayDis = 0.5f;
    [SerializeField] private float rayDisDown = 1f;
    public RaycastHit2D rightHit, leftHit, upHit, downHit;
    [SerializeField]bool Grounded;
    [SerializeField] bool useGravity;

    private void Awake()
    {
        isControllable = true;
        useGravity = true;
    }

    private void Update()
    {
        RayDetector();
        Grounded = downHit;
        InputDetector();
        FlipPlayerDir();
        

        CalculateWalkSpeed();
        CalculateJumpApex();
        Gravity();
        JumpOptimation();

        SlopeCheck();
        Jump();
        WallEvent();
        CharacterMove();
        Physics2D.SyncTransforms();
    }

    private void FixedUpdate()
    {
       
    }

    private void CharacterMove()
    {
        HandleMove();
        transform.position += (Vector3) velocity * Time.fixedDeltaTime;
    }

    private void HandleMove()
    {
        if (downHit && isOnSlope && !isJumping && canMoveOn)
        {
            velocity = slopeNormalPerp * -currentSpeedX;
        }
        else
        {
            velocity = new Vector2 (currentSpeedX, currentSpeedY);
        }
    }

    private void CalculateWalkSpeed()
    {
        if (isControllable)
        {
            if (inputDir.x != 0)
            {
                // speed acceleration when input
                currentSpeedX += inputDir.x * moveAcceleration * Time.deltaTime;
                currentSpeedX = Mathf.Clamp(currentSpeedX, -moveClamp, moveClamp);
            }
            else
            {
                //deacceleration  when not input
                currentSpeedX = Mathf.MoveTowards(currentSpeedX, 0, deAcceleration * Time.deltaTime);
            }
            //check wall
            if (currentSpeedX > 0 && rightHit || currentSpeedX < 0 && leftHit)
            {
                currentSpeedX = 0;
            }
        }
    }

    /// <summary>
    /// Jump
    /// </summary>
    #region Jump
    private void CalculateJumpApex()
    {
        if (!downHit)
        {
            apexPoint = Mathf.InverseLerp(jumpApexThreshold, 0, Mathf.Abs(currentSpeedY));
            fallGravity = Mathf.Lerp(minFallGravity, maxFallGraviyt, apexPoint);
        }
        else
        {
            apexPoint = 0;
        }
    }

    private void Jump()
    {
        if (isControllable && !isWallJumping)
        {
            if (jumpInputBufferTimer > 0.01f && coyoteJumpTimer > 0.01f )
            {
                isJumping = true;
                currentSpeedY = jumpHight;
                jumpInputBufferTimer = -0.1f;
            }
            if (!downHit && jumpInputUp && currentSpeedY > 0)
            {
                isJumpEarlyUp = true;
            }

        }
    }

    private void JumpOptimation()
    {
        // coyote Jump
        if (downHit)
        {
            coyoteJumpTimer = coyoteJump;
        }
        else
        {
            coyoteJumpTimer -= Time.deltaTime;
            coyoteJumpTimer = Mathf.Clamp(coyoteJumpTimer, -0.2f, coyoteJump);
        }

        //Inputbuffer
        if (jumpInputDown)
        {
            slopeDownAngleOld = 0;
            jumpInputBufferTimer = jumpInputBuffer;
        }
        else
        {
            jumpInputBufferTimer -= Time.deltaTime;
            //jumpInputBufferTimer = Mathf.Clamp(jumpInputBufferTimer, 0, jumpInputBuffer);
            if (jumpInputBufferTimer < 0) jumpInputBufferTimer = -0.1f;

        }
    }

    private void Gravity()
    {
        if (!useGravity)
        {
            return;
        }
        if (downHit)
        {
            if (currentSpeedY < 0)
            {
                currentSpeedY = 0;
                
                transform.position = (Vector3)downHit.point + new Vector3(0, 0.9f, 0);
                isJumpEarlyUp = false;
            }
            return;
        }
        else
        {
            isJumping = false;
            float fallspeed;
            if (isJumpEarlyUp && currentSpeedY > 0)
            {
                fallspeed = fallGravity * jumpEarlyMul;
            }
            else
            {
                fallspeed = fallGravity;
            }
            currentSpeedY -= fallspeed * Time.deltaTime;
            if (currentSpeedY < gravityClamp) currentSpeedY = gravityClamp;

        }
    }
    #endregion

    /// <summary>
    /// Wall slide & Jump
    /// </summary>
    #region wall
    private void WallEvent()
    {
        WallSlide();
        WallJump();
    }

    bool isWalled => rightHit || leftHit;

    private void WallSlide()
    {
        if (isWalled && !downHit && inputDir.x != 0f)
        {
            isWallSliding = true;
            currentSpeedY = Mathf.Clamp(currentSpeedY, -wallSlideSpeed, float.MaxValue);
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpDir = playerDir == CharDir.Right ? -1 : 1;
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(StopWallJump));
        }
        else
        {
            wallJumpTimer -= Time.deltaTime;
            if (wallJumpTimer < 0) wallJumpTimer = -0.1f;
        }

        if (jumpInputDown && wallJumpTimer > 0)
        {
            useGravity = false;
            isWallJumping = true;
            isControllable = false;
            currentSpeedX = wallJumpDir * wallJumpPower.x;
            currentSpeedY = wallJumpPower.y;
            wallJumpTimer = -0.1f;

            //flip();
        }

        Invoke(nameof(StopWallJump), wallJumpDuration);
    }
    #endregion
    
    private void StopWallJump()
    {
        
        isWallJumping = false;
        useGravity = true;
        isControllable = true;
    }


    private void Dash()
    {

    }

    /// <summary>
    /// slope move
    /// </summary>
    #region Slope
    private void SlopeCheck()
    {
        SlopeCheckVertial();
        //SlopeCheckHorizontal();
    }

    private void SlopeCheckHorizontal()
    {
        RaycastHit2D hitFront = Physics2D.Raycast(transform.position, transform.right, slopeCheckDis, 1 << 10);
        RaycastHit2D hitBack = Physics2D.Raycast(transform.position, -transform.right, slopeCheckDis, 1 << 10);

        if (hitFront || hitBack)
        {
            isOnSlope = true;
            RaycastHit2D hit = hitFront ? hitFront : hitBack;
            slopeSideAngle = Vector2.Angle(hit.normal, Vector2.up);
        }
        else
        {
            slopeSideAngle = 0.0f;
            isOnSlope = false;
        }
    }

    private void SlopeCheckVertial()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, slopeCheckDis, 1 << 10);

        if (hit&& hit.collider.tag == "Slope")
        {
            Debug.DrawRay(hit.point, slopeNormalPerp, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.green);

            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);
            isOnSlope = true;
            //if (slopeDownAngle != slopeDownAngleOld)
            //{
            //    isOnSlope = true;
            //}
            slopeDownAngleOld = slopeDownAngle;
            canMoveOn = slopeDownAngle > maxSlopeAngle ? false : true;
        }
        else
        {
            slopeDownAngle = 0;
            isOnSlope = false;
        }

       
        
    }
    #endregion


    private void FlipPlayerDir()
    {
        if (inputDir.x > 0)
        {
            playerDir = CharDir.Right;
        }
        else if (inputDir.x < 0)
        {
            playerDir = CharDir.Left;
        }
    }


    /// <summary>
    /// Detector
    /// </summary>
    public void InputDetector()
    {
        if (!isControllable)
        {
            return;
        }
        inputDir.x = PlayerInput._instance.HorizontalInput;
        inputDir.y = PlayerInput._instance.VerticalInput;
        jumpInputDown = PlayerInput._instance.jumpBtnDown;
        jumpInputUp = PlayerInput._instance.jumpBtnUp;
    }

    private void RayDetector()
    {

        rightHit = Physics2D.Raycast(transform.position, Vector2.right, rayDis, (1 << 6));
        leftHit = Physics2D.Raycast(transform.position, Vector2.left, rayDis, (1 << 6));
        upHit = Physics2D.Raycast(transform.position, Vector2.up, rayDis, (1 << 10));
        downHit = Physics2D.Raycast(transform.position, Vector2.down, rayDisDown, (1 << 10 | 1 << 6));
        Debug.DrawLine(transform.position, transform.position + Vector3.down * rayDisDown, Color.red, 1);
        Debug.DrawLine(transform.position, transform.position + Vector3.right * rayDis, Color.red, 1);
       
    }
}
