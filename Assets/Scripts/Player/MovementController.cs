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
    [SerializeField] private float currentSpeedX;
    [SerializeField] private float currentSpeedY;
    public CharDir playerDir = CharDir.Right;

    [Header("JUMP")]
    //Character Basic data
    [SerializeField] private float jumpHight = 30f;
    [SerializeField] private float doubleJumpHight = 10f;
    [SerializeField] private float jumpApexThreshold = 10f;
    [SerializeField] private float jumpEarlyMul = 3f;
    [SerializeField] private float coyoteJump = 0.2f;
    [SerializeField] private float jumpInputBuffer = 0.2f;
    [SerializeField] private float coyoteJumpTimer;
    [SerializeField] private float jumpInputBufferTimer;
    private float apexPoint;
    private bool isJumping;
    private bool canDoubleJump;
    [SerializeField] private AudioSource jumpSound;

    [Header("DASH")]
    [SerializeField] private Vector2 dashDir;
    [SerializeField] private float dashTime = 0.3f;
    [SerializeField] private bool isDashing;
    [SerializeField] private bool canDash;
    [SerializeField] private float rechargeTime = 0.2f;

    [Header("Wall Jump & Wall Slide")]
    [SerializeField] public bool isWallSliding;
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private float wallJumpTime = 0.2f;
    [SerializeField] private float wallJumpWindow = 0.1f;
    [SerializeField] private Vector2 wallJumpPower = new Vector2(8f, 16f);
    private float wallJumpWindowTimer;
    private bool isWallJumping;
    private float wallJumpTimer;

    [Header("GRAVITY")]
    [SerializeField] private float gravityClamp = -30f;
    [SerializeField] private float minFallGravity = 80f;
    [SerializeField] private float maxFallGraviyt = 120f;
    [SerializeField] public float fallGravity;
    private bool isJumpEarlyUp;

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
    [SerializeField] private bool isOnSlope;
    private bool canMoveOn;
    private float slopeDownAngleOld;
    private float slopeSideAngle;

    [Header("RAY")]
    //RayCast
    [SerializeField] private float rayDis = 0.5f;
    [SerializeField] private float rayDisDown = 1f;
    [SerializeField] private float interactDecDis = 0.8f;
    private RaycastHit2D rightHit, leftHit, upHit, downHit;
    private RaycastHit2D interacte;

    [Header("Player State")]
    [SerializeField] private bool grounded;
    [SerializeField] private bool rightRay;
    [SerializeField] private bool useGravity;
    [SerializeField] private bool isControllable;
    [SerializeField] private bool isAlive;
    [SerializeField] private bool canInput;

    //Input
    private bool jumpInputUp;
    private bool jumpInputDown;

    //bool isTouchWall => currentSpeedX > 0 && rightHit && !isOnSlope || currentSpeedX < 0 && leftHit && !isOnSlope;
    bool isTouchWall => currentSpeedX > 0 && rightHit || currentSpeedX < 0 && leftHit;

    // reference
    public bool IsWallSliding => isWallSliding;
    public bool IsWallJumping => isWallJumping;
    public Vector2 Velocity => velocity;
    public bool IsJumping => isJumping;
    public bool Grounded => grounded;

    public bool IsControllable => isControllable;

    private void Awake()
    {
        isControllable = false;
        canInput = true;
        useGravity = true;
        canDash = true;
    }

    public void GameStart()
    {
        isControllable = true;
    }

    private void Update()
    {    
        if (!isControllable)
        {
            return;
        }
        RayDetector();
        InputDetector();
        FlipPlayerDir();

#if UNITY_EDITOR
        grounded = downHit;
        rightRay = rightHit;
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            //FlipDir();
            //Recharge();
            Dash();
        }
#endif
        CalculateWalkSpeed();
        CalculateJumpApex();
        Gravity();
        JumpOptimation();
        Dash();

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
        if (downHit && isOnSlope && !isJumping && canMoveOn && !isDashing)
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
        if (!isWallJumping && !isDashing)
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
        if (!isWallSliding)
        {
            if (!isDashing)
            {
                if (jumpInputBufferTimer > 0.01f && coyoteJumpTimer > 0.01f)
                {
                    jumpSound.Play();
                    isJumping = true;
                    currentSpeedY = jumpHight;
                    jumpInputBufferTimer = -0.1f;
                }
                if (!downHit && jumpInputUp && currentSpeedY > 0)
                {
                    isJumpEarlyUp = true;
                }
            }
            if (velocity.y < 0)
            {
                DoubleJump();
            }

        }
    }

    private void DoubleJump()
    {
        if (jumpInputBufferTimer > 0.01f && canDoubleJump)
        {
            canDoubleJump = false;
            StopAllCoroutines();
            useGravity = true;
            isDashing = false;
            isJumping = true;

            currentSpeedY = doubleJumpHight;
        }
    }

    private void JumpOptimation()
    {
        // coyote Jump
        if (downHit)
        {
            coyoteJumpTimer = coyoteJump;
            canDoubleJump = true;
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
            canDash = true;
            if (currentSpeedY < 0)
            {
                currentSpeedY = 0;
                isJumping = false;
                transform.position = (Vector3)downHit.point + new Vector3(0, 0.9f, 0);
                isJumpEarlyUp = false;
            }
            return;
        }
        else
        {
            //isJumping = false;
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

    #region DASH

    private bool borrrowPower;
    private bool dash => canDash && downHit && PlayerInput._instance.Dash && !isWallSliding;
    private bool borrow => borrrowPower && PlayerInput._instance.Dash && !isWallSliding;

    private void Dash()
    {
        if ( dash|| borrow )
        {
            Debug.Log("在冲了在冲了");
            float inputx = inputDir.x > 0 ? 1 : -1;
            float dirX = inputDir.x != 0 ? inputx : transform.localScale.x;
            float dirY = inputDir.x != 0 ? 1 : 0;
            Debug.Log("X:"+dirX +"Y:"+dirY);
            StartCoroutine(Dashing(dirX,dirY));
            jumpSound.Play();
        }       
    }

    IEnumerator Dashing(float dirX, float dirY)
    {
        isDashing = true;
        useGravity = false;
        currentSpeedX = dirX * dashDir.x;
        currentSpeedY = dirY * dashDir.y;
        yield return new WaitForSeconds(dashTime);

        canDoubleJump = true;
        canDash = false;
        useGravity = true;
        isDashing = false;
    }

    // Dash 接受后，

    public void RechargeWindow()
    {
        Debug.Log("Recharge");
        StopCoroutine(WindowCounter());
        StartCoroutine(WindowCounter());
        
    }

    private IEnumerator WindowCounter()
    {
        borrrowPower = true;
        yield return new WaitForSeconds(rechargeTime);
        borrrowPower = false;
        
    }
    #endregion

    /// <summary>
    /// Wall slide & Jump
    /// </summary>
    #region WALL
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
            jumpSound.Play();
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
        SlopeCheckHorizontal();
    }

    private void SlopeCheckHorizontal()
    {
        RaycastHit2D hitFront = Physics2D.Raycast(transform.position, transform.right, slopeCheckDis, 1 << 7);
        RaycastHit2D hitBack = Physics2D.Raycast(transform.position, -transform.right, slopeCheckDis, 1 << 7);

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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, slopeCheckDis, 1 << 7);

        if (hit/*&& hit.collider.tag == "Slope"*/)
        {
            Debug.DrawRay(hit.point, slopeNormalDir, Color.green);
            Debug.DrawRay(hit.point, hit.normal, Color.white);
            slopeNormalDir = Vector2.Perpendicular(hit.normal).normalized;
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);
            isOnSlope = true;
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
        rightHit = Physics2D.Raycast(transform.position, Vector2.right, rayDis, (1 << 7));
        leftHit = Physics2D.Raycast(transform.position, Vector2.left, rayDis, (1 << 7));
        upHit = Physics2D.Raycast(transform.position, Vector2.up, rayDis, (1 << 7));
        downHit = Physics2D.Raycast(transform.position, Vector2.down, rayDisDown, (1 << 7));
        Debug.DrawLine(transform.position, transform.position + Vector3.down * rayDisDown, Color.red, 1);
        Debug.DrawLine(transform.position, transform.position + Vector3.right * rayDis, Color.red, 1);

        //interacte = Physics2D.Raycast(transform.position, Vector2.right, interactDecDis, (1 << 8));
       
    }

 
}
