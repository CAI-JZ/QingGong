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
    [SerializeField] private Vector2 velocity;

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

    private float apexPoint;
    private bool isJumping;
    [SerializeField] private bool isRecharging;

    [Header("Dash")]
    private float dashPower;
    private Vector2 dashDir;
    private float dashTime = 0.3f;
    private float dashTimer;
    private bool isDashing;
    [SerializeField] private float rechargeTime = 0.2f;
    [SerializeField] private float rechargeTimer;
    private bool jumpInputUp;

    [Header("Wall Jump & Wall Slide")]
    [SerializeField] public bool isWallSliding;
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private float wallJumpTime = 0.2f;
    [SerializeField] private float wallJumpWindow = 0.1f;
    [SerializeField] private float wallJumpWindowTimer;
    private bool isWallJumping;
    [SerializeField] private float wallJumpTimer;
    [SerializeField] private Vector2 wallJumpPower = new Vector2(8f, 16f);

    [Header("GRAVITY")]
    [SerializeField] private float gravityClamp = -30f;
    [SerializeField] private float minFallGravity = 80f;
    [SerializeField] private float maxFallGraviyt = 120f;
    private bool isJumpEarlyUp;
    [SerializeField] public float fallGravity;

    //Moves
    [Header("MOVE")]
    [SerializeField] public float moveAcceleration = 50f;
    [SerializeField] public float deAcceleration = 50f;
    [SerializeField] public float moveClamp = 13f;


    [Header("SlopeMove")]
    [SerializeField] private float slopeCheckDis;
    [SerializeField] private float maxSlopeAngle;
    private Vector2 slopeNormalDir;
    [SerializeField] private float slopeDownAngle;
    private float slopeDownAngleOld;
    private float slopeSideAngle;
    [SerializeField] private bool isOnSlope;
    private bool canMoveOn;

    [Header("RAY")]
    //RayCast
    [SerializeField] private float rayDis = 0.5f;
    [SerializeField] private float rayDisDown = 1f;
    public RaycastHit2D rightHit, leftHit, upHit, downHit;

    [Header("Player State")]
    [SerializeField] private bool grounded;
    [SerializeField] private bool useGravity;
    [SerializeField] private bool isControllable;
    [SerializeField] private bool isAlive;
    [SerializeField] private bool canInput;

    bool isTouchWall => currentSpeedX > 0 && rightHit && !isOnSlope || currentSpeedX < 0 && leftHit && !isOnSlope;

    // reference
    public bool IsWallSliding => isWallSliding;
    public bool IsWallJumping => isWallJumping;
    public Vector2 Velocity => velocity;


    private void Awake()
    {
        isAlive = true;
        isControllable = true;
        canInput = true;
        useGravity = true;
    }

    private void Update()
    {
        if (!isAlive)
        {
            return;
        }

        RayDetector();
        InputDetector();
        FlipPlayerDir();

#if UNITY_EDITOR
        grounded = downHit;
        if (Input.GetKeyDown(KeyCode.C))
        {
            //FlipDir();
            Recharge();
        }
#endif
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


    private void CharacterMove()
    {
        HandleMove();
        transform.position += (Vector3)velocity * Time.deltaTime;
    }

    private void HandleMove()
    {
        // slope walk
        if (downHit && isOnSlope && !isJumping && canMoveOn)
        {
            velocity = slopeNormalDir * -currentSpeedX;
        }
        else
        {
            velocity = new Vector2(currentSpeedX, currentSpeedY);
        }
    }

    private void CalculateWalkSpeed()
    {
        if (isControllable && !isWallJumping)
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
            if (isTouchWall)
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
        if (isControllable && !isWallSliding)
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
        if (downHit && downHit.collider.tag != "Wall")
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

    private void Dash()
    {
        dashTimer = dashTime;
    }

    IEnumerator Dashing()
    {
        isDashing = true;
        while (dashTimer >0)
        {
            dashTimer -= Time.deltaTime;
            yield return null;
        }
        dashTimer = -0.1f;

        isDashing = false;
    }

    private void Recharge()
    {
        rechargeTimer = rechargeTime;
        isRecharging = true;
        StopCoroutine(RechargeWindow());
        StartCoroutine(RechargeWindow());
    }

    private IEnumerator RechargeWindow()
    {
        while (rechargeTimer > 0)
        {
            rechargeTimer -= Time.deltaTime;
            yield return null;
        }
        rechargeTimer = -0.1f;
        isRecharging = false;
    }



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
        isWallSliding = isWalled && !downHit && inputDir.x != 0f ? true : false;

        if (isWallSliding)
        {
            Debug.Log("我碰到了！");
            wallJumpWindowTimer = wallJumpWindow;
            float normalDir = rightHit ? -1 : 1;
            if (inputDir.x == normalDir)
            {
                return;
            }

            velocity = Vector3.zero;
            currentSpeedX = 0;
            currentSpeedY = Mathf.Clamp(currentSpeedY, wallSlideSpeed, float.MaxValue);
        }
        else
        {
            wallJumpWindowTimer -= Time.deltaTime;
            if (wallJumpWindowTimer < 0) wallJumpWindowTimer = -0.1f;
        }
    }

    private void WallJump()
    {
        if (isWallSliding && jumpInputDown && wallJumpWindowTimer>0)
        {
            wallJumpTimer = wallJumpTime;
        }
        else
        {
            wallJumpTimer -= Time.deltaTime;
            if (wallJumpTimer < 0) wallJumpTimer = -0.1f;
        }

        if (wallJumpTimer > 0)
        {
            if (!isWalled) { return; }
            float normal = rightHit ? rightHit.normal.x : leftHit.normal.x;

            isWallJumping = true;
            isWallSliding = false;
            currentSpeedX = Mathf.Lerp(currentSpeedX, normal * wallJumpPower.x, wallJumpPower.x / wallJumpTime);
            currentSpeedY = Mathf.Lerp(currentSpeedY, wallJumpPower.y, wallJumpPower.y / wallJumpTime);
            Debug.Log("Wall Jump");
        }
        else
        {
            isWallJumping = false;
        }      
    }
#endregion

    /// <summary>
    /// slope move 这里要改，改成按住一个方向键，就可以左右反复横跳，如果没有按，只会给一个后推力。
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
            Debug.DrawRay(hit.point, slopeNormalDir, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.green);

            slopeNormalDir = Vector2.Perpendicular(hit.normal).normalized;
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
        if (velocity.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (velocity.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void FlipDir()
    {
        float dir = transform.localScale.x * -1;
        transform.localScale = new Vector3(dir, 1, 1);
    }


    /// <summary>
    /// Detector
    /// </summary>
    public void InputDetector()
    {
        if (!canInput)
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

        rightHit = Physics2D.Raycast(transform.position, Vector2.right, rayDis, (1 << 6 | 1 << 10));
        leftHit = Physics2D.Raycast(transform.position, Vector2.left, rayDis, (1 << 6 | 1 << 10));
        upHit = Physics2D.Raycast(transform.position, Vector2.up, rayDis, (1 << 10));
        downHit = Physics2D.Raycast(transform.position, Vector2.down, rayDisDown, (1 << 10 | 1 << 6));
        Debug.DrawLine(transform.position, transform.position + Vector3.down * rayDisDown, Color.red, 1);
        Debug.DrawLine(transform.position, transform.position + Vector3.right * rayDis, Color.red, 1);
       
    }

 
}
