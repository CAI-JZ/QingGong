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
    [SerializeField] private float staminaMaxValue;
    [SerializeField] private float currentStamina;
    [SerializeField] private float wallStaminaMul;
    public CharDir playerDir = CharDir.Right;

    [Header("JUMP")]
    //Character Basic data
    [SerializeField] private float jumpHight = 30f;
    [SerializeField] private float doubleJumpHight = 10f;
    [SerializeField] private float jumpApexThreshold = 10f;
    [SerializeField] private float jumpEarlyMul = 3f;
    [SerializeField] private float coyoteJump = 0.2f;
    [SerializeField] private float jumpInputBuffer = 0.2f;
    private float coyoteJumpTimer;
    private float jumpInputBufferTimer;
    private float apexPoint;
    private bool isJumping;
    private bool canDoubleJump;

    [Header("DASH")]
    [SerializeField] private Vector2 dashDir;
    [SerializeField] private float dashTime = 0.3f;
    [SerializeField] private bool isDashing;
    [SerializeField] private bool canDash;
    [SerializeField] private float rechargeTime = 0.2f;
    [SerializeField] private Transform rechargeCheck;
    [SerializeField] private float rechargeCheckDis;
    private float dashTimer;

    [Header("Wall Jump & Wall Slide")]
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private float wallJumpTime = 0.2f;
    [SerializeField] private Vector2 wallJumpPower = new Vector2(8f, 16f);
    [SerializeField]private bool isWallSliding;
    private bool isWallJumping;
    private bool canWallSlide;
    private float wallJumpWindow = 0.1f;
    private float currentWallSpeed;
    private float wallSpeedTerp;
    private float wallJumpWindowTimer;
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

    [Header("SLOPE MOVE")]
    [SerializeField] private float maxSlopeAngle;
    private Vector2 slopeNormalDir;
    [SerializeField] private float slopeDownAngle;
    [SerializeField] private bool isOnSlope;
    private bool canMoveOn;
    private float slopeSideAngle;

    [Header("BAMBOO")]
    [SerializeField] private float bambooDecRadis;
    [SerializeField] private Transform bambooDetect;
    [SerializeField] private Vector2 bambooJumpPower;
    [SerializeField] private float bambooJumpDuration;
    [SerializeField] private float bendMul;
    private Vector2 bambooNormalDir;
    private float bambooJumpTimer = -0.1f;
    private bool isBambooJumping;
    private bool isWalkBamboo;
    private bool isStandOnBamboo;
    private RaycastHit2D isbamboo;
    private bool isTouchBamboo;

    [Header("RAY")]
    //RayCast
    [SerializeField] private float rayDis = 0.5f;
    [SerializeField] private float rayDisDown = 1f;
    [SerializeField] private Transform climbableCheck;
    private RaycastHit2D rightHit, leftHit, upHit, downHit;
    private RaycastHit2D isClimbable;


    [Header("Player State")]
    [SerializeField] private bool useGravity;
    [SerializeField] private bool isControllable;
    [SerializeField] private bool canInput;

    //Input
    private bool jumpInputUp;
    private bool jumpInputDown;
    private bool dashInput;

    bool isTouchWall => currentSpeedX > 0 && rightHit && !isOnSlope || currentSpeedX < 0 && leftHit && !isOnSlope;

    // reference
    public bool IsWallSliding => isWallSliding;
    public bool IsWalkBamboo => isWalkBamboo;
    public bool IsWallJumping => isWallJumping;
    public bool IsDashing => isDashing;
    public Vector2 Velocity => velocity;
    public bool IsJumping => isJumping;
    public bool Grounded => downHit;
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


    [SerializeField] GameObject test;
    private void Update()
    {
        if (!isControllable)
        {
            return;
        }
        RayDetector();
        InputDetector();
        FlipPlayerDir();

        CalculateWalkSpeed();
        CalculateJumpApex();
        Gravity();
        JumpOptimation();
        DashEvent();

        SlopeCheck();
        Jump();
        WallEvent();
        BambooEvent();
        CharacterMove();
        Physics2D.SyncTransforms();
    }


    private void ResetPlayerData()
    {
        currentStamina = staminaMaxValue;
        currentWallSpeed = wallSlideSpeed;
        canDash = true;
        climbHight = 0;

    }

    #region Run
    private void CharacterMove()
    {
        HandleMove();
        transform.position += (Vector3)velocity * Time.deltaTime;
    }

    private void HandleMove()
    {
        if (isStandOnBamboo && !isJumping && !isDashing)
        {
            velocity = bambooNormalDir * -currentSpeedX;
        }
        // slope walk
        else if (downHit && isOnSlope && !isJumping && canMoveOn && !isDashing)
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
        if (!isWallJumping && !isDashing && !isBambooJumping)
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
    #endregion

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
            ResetPlayerData();
            if (currentSpeedY <= 0)
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
    private bool dash => dashInput && canDash && downHit && !isWallSliding ;
    private bool borrow => borrrowPower && !isWallSliding;

    private bool doubleCheck => rightHit && currentSpeedX >0 && !isOnSlope || leftHit && currentSpeedX <0 && !isOnSlope;

    //增加一个检测、增加一个banboo的晃动

    private void DashEvent()
    {
        RecharegeCheck();
        DashInput();
        DashHandle();
        DoubleJump();
    }

    private void RecharegeCheck()
    {
        if (!dashInput)
        {
            borrrowPower = false;
            return;
        }
        else
        { 
            RaycastHit2D hit = Physics2D.Raycast(rechargeCheck.position, Vector2.right, rechargeCheckDis, (1 << 6)|(1 << 8));
            
            if (!hit) {  return; }
            else
            {
                IBorrow ib = null;
                if (hit.collider.tag == "Bamboo")
                {
                    ib = hit.collider.transform.parent.GetComponentInChildren<IBorrow>();
                }
                if (hit.collider.tag == "Recharge")
                {
                    ib = hit.collider.gameObject.GetComponent<IBorrow>();
                }
                if (ib == null)
                {
                    return;
                }
                else
                {
                    ib.BorrowPower(inputDir.x);
                    borrrowPower = true;
                }
            }
        }
    }

    private void DashInput()
    {
        if (dash || borrow)
        {
            isDashing = true;
            dashTimer = dashTime;
        }
        else
        {
            dashTimer -= Time.deltaTime;
            dashTimer = Mathf.Clamp(dashTimer, -0.1f, dashTime);
        }
    }

    private void DashHandle()
    {
        if (!isDashing)
        {
            return;
        }
        DoubleCheck();
       
        
        if (dashTimer < 0)
        {
            EndDash();
            return;
        }
        else
        {
            float inputx = inputDir.x > 0 ? 1 : -1;
            float dirX = inputDir.x != 0 ? inputx : transform.localScale.x;
            float dirY = 1;

            useGravity = false;
            currentSpeedX = dirX * dashDir.x;
            currentSpeedY = dirY * dashDir.y;
        }
    }

    private void DoubleJump()
    {
        if (jumpInputBufferTimer > 0.01f && canDoubleJump)
        {
            canDoubleJump = false;
            useGravity = true;
            isDashing = false;
            isJumping = true;

            currentSpeedY = doubleJumpHight;
        }
    }

    private void DoubleCheck()
    {
        if (doubleCheck)
        {
            currentSpeedX = 0;
            currentSpeedY = 0;
            EndDash();
            dashTimer = -0.1f;
            return;
        }
    }

    private void EndDash()
    {
        useGravity = true;
        canDoubleJump = true;
        currentStamina = staminaMaxValue;
        canDash = false;
        borrrowPower = false;
        isDashing = false;
    }
    #endregion

    private void BambooEvent()
    {
        BambooWalk();
        BambooJump();
    }


    [SerializeField] float power = 0;
    [SerializeField] float climbHight = 0;
    [SerializeField] float hightMul = 4;
    [SerializeField] float hightMaxValue = 3;
    private void BambooWalk()
    {
        if (!isbamboo && !isStandOnBamboo || currentStamina <=0)
        {
            isWalkBamboo = false;
            power = 0;
            return;
        }
        if (inputDir.y > 0 && inputDir.x != 0)
        {
            power += Time.deltaTime * hightMul;
            climbHight += Time.deltaTime * hightMul;
            climbHight = Mathf.Clamp(climbHight, 0, hightMaxValue);
            RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.down * 0.8f, Vector2.down, 1f, (1 << 6));
            Debug.DrawLine(transform.position + Vector3.down * 0.7f, transform.position + Vector3.down * (0.5f + 0.8f), Color.green, 1);

            
            if (!isbamboo || !isbamboo.collider.transform.parent.GetChild(0).TryGetComponent<Bamboo>(out Bamboo b))
            {
                Debug.Log("找不到对象");
                isWalkBamboo = false;
                return;
            }

            b.AddForce(power, inputDir.x);
            WallStaminaDecrease(wallStaminaMul);
            isWalkBamboo = true;
            if (!hit)
            {
                currentSpeedX = 0;
                currentSpeedY = wallSlideSpeed;
                isStandOnBamboo = false;
                
            }
            else
            {
                isStandOnBamboo = true;
                bambooNormalDir = Vector2.Perpendicular(hit.normal).normalized;
                Debug.DrawRay(hit.point, bambooNormalDir, Color.red);
                Debug.DrawRay(hit.point, hit.normal, Color.white);
                Debug.Log("沿着竹子爬。");
            }
            
        }
        else
        {
            isWalkBamboo = false;
            isBambooJumping = false;
        }
    }

    private void BambooJump()
    {
        if (isWalkBamboo && jumpInputDown)
        {
            bambooJumpTimer = bambooJumpDuration;
        }
        else
        {
            bambooJumpTimer -= Time.deltaTime;
            bambooJumpTimer = Mathf.Max(bambooJumpTimer, -0.1f);
        }
        if (bambooJumpTimer > 0)
        {
            isBambooJumping = true;
            float mul = climbHight > 1 ? climbHight * bendMul : 1;
            float normal = inputDir.x > 0 ? -1 : 1;
            currentSpeedX = Mathf.Lerp(currentSpeedX, normal * bambooJumpPower.x * mul, bambooJumpTimer);
            currentSpeedY = Mathf.Lerp(currentSpeedY, bambooJumpPower.y* mul, bambooJumpTimer);
        }
        else
        {
            isBambooJumping = false;
        }
    }

    /// <summary>
    /// Wall slide & Jump
    /// </summary>
    #region WALL
    private void WallEvent()
    {
        WallSlide();
        WallJump();
    }

    bool isWalled => rightHit || leftHit || isClimbable ;

    private void WallSlide()
    {
        canWallSlide = isWalled && inputDir.x != 0f && inputDir.y >0 ? true : false;

        if (canWallSlide && !isOnSlope)
        {
            if ( currentStamina < 0)
            {
                isWallSliding = false;
                return;
            }

            isWallSliding = true;
            //velocity = Vector3.zero;
            currentSpeedX = 0;
            //currentSpeedY = Mathf.Clamp(currentSpeedY, wallSlideSpeed, float.MaxValue);
            currentSpeedY = wallSlideSpeed;
            //currentSpeedY = currentWallSpeed;
            //currentWallSpeed = Mathf.Lerp(currentWallSpeed,0, wallSpeedTerp);
            WallStaminaDecrease(wallStaminaMul);
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallStaminaDecrease(float decMul)
    {
        currentStamina -= Time.deltaTime * decMul;
    }

    private void WallJump()
    {
        if (isWallSliding && jumpInputDown)
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

            float normal = 1;
            if (isClimbable)
            {
                normal = isClimbable.normal.x;
            }
            else
            { 
                normal = rightHit ? rightHit.normal.x : leftHit.normal.x; 
            }

            isWallJumping = true;
            isWallSliding = false;
            currentSpeedX = Mathf.Lerp(currentSpeedX, normal * wallJumpPower.x, wallJumpPower.x / wallJumpTime);
            currentSpeedY = Mathf.Lerp(currentSpeedY, wallJumpPower.y, wallJumpPower.y / wallJumpTime);
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
        if (rightHit || leftHit)
        {
            RaycastHit2D hit = rightHit ? rightHit : leftHit;
            slopeSideAngle = Vector2.Angle(hit.normal, Vector2.up);
            isOnSlope = slopeSideAngle < 90 ? true : false;
        }
        else
        {
            slopeSideAngle = 0.0f;
            isOnSlope = false;
        }
    }

    private void SlopeCheckVertial()
    {
        RaycastHit2D hit = downHit;

        if (!hit) return;
        slopeNormalDir = Vector2.Perpendicular(hit.normal).normalized;
        slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);
        if (slopeDownAngle > 0)
        {
            isOnSlope = true;
            canMoveOn = slopeDownAngle > maxSlopeAngle ? false : true;
        }
        else
        {
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
        dashInput = PlayerInput._instance.Dash;
    }

    private void RayDetector()
    {
        rightHit = Physics2D.Raycast(transform.position+Vector3.down * 0.8f, Vector2.right, rayDis, (1 << 7));
        leftHit = Physics2D.Raycast(transform.position + Vector3.down * 0.8f, Vector2.left, rayDis, (1 << 7));
        upHit = Physics2D.Raycast(transform.position, Vector2.up, rayDis, (1 << 7));
        downHit = Physics2D.Raycast(transform.position + Vector3.down * 0.8f, Vector2.down, rayDisDown, (1 << 7));
        isbamboo = Physics2D.Raycast(bambooDetect.position, Vector2.right, bambooDecRadis, (1 << 6));
        isClimbable = Physics2D.Raycast(climbableCheck.position, Vector2.right, bambooDecRadis, (1 << 8));

#if UNITY_EDITOR
        //Debug.DrawLine(transform.position + Vector3.down * 0.7f, transform.position + Vector3.down * (rayDisDown+0.8f), Color.red, 1);
        Debug.DrawLine(transform.position+ Vector3.down * 0.8f, transform.position + Vector3.down * 0.8f + Vector3.right * rayDis, Color.red, 1);
        Debug.DrawLine(transform.position + Vector3.down * 0.8f, transform.position + Vector3.down * 0.8f + Vector3.left * rayDis, Color.red, 1);
        //Debug.DrawLine(bambooDetect.position, bambooDetect.position + Vector3.right * bambooDecRadis, Color.green, 1);
        Debug.DrawLine(rechargeCheck.position, rechargeCheck.position + Vector3.right * rechargeCheckDis, Color.red, 1);
#endif
    }

 
}
