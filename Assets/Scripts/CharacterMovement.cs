using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    //Move
    [Header("BASIC DATA")]
    [SerializeField]
    Vector3 Velocity;
    float moveDir;

    [Header("JUMP")]
    //Character Basic data
    [SerializeField] private float jumpHight = 30f;
    [SerializeField] private float apexPoint;
    [SerializeField] private float jumpApexThreshold = 10f;
    [SerializeField] private float jumpEarlyMul = 3f;
    private float coyoteJump = 0.2f;
    [SerializeField]private float coyoteJumpTimer;
    private float jumpInputBuffer = 0.2f;
    [SerializeField]private float jumpInputBufferTimer;
    private bool jumpInputDown;
    private bool jumpInputUp;

    //Gravity
    public float fallGravity;
    [SerializeField]private float gravityClamp = -30f;
    [SerializeField]private float minFallGravity = 80f;
    [SerializeField]private float maxFallGraviyt = 120f;
    private bool isJumpEarlyUp;

    //Moves
    [Header("MOVE")]
    public float moveAcceleration = 50f;
    public float deAcceleration = 50f;
    public float moveClamp = 13f;



    [Header("RAY")]
    //RayCast
    [SerializeField] private float rayDis = 0.5f;
    [SerializeField] private bool rightRay, leftRay, upRay, downRay;
    private RaycastHit rightInfo, leftInfo, upInfo, downInfo;
    [Range(0, 1)]
    
    //GroundCheck
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField] private bool isGround;

    private void Awake()
    {

    }

    private void FixedUpdate()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RayDetector();
        isGround = GroundCheck();
        InputDetector();
        JumpOptimation();

        CalculateWalk();
        CalculateJumpApex();
        Gravity();
        Jump();
        CharacterMove();

    }

    private void LateUpdate()
    {
        
    }


    private void CalculateWalk()
    {
        if (moveDir != 0)
        {
            // speed acceleration when input
            Velocity.x += moveDir * moveAcceleration * Time.deltaTime;
            Velocity.x = Mathf.Clamp(Velocity.x, -moveClamp, moveClamp);
        }
        else
        {
            //deacceleration  when not input
            Velocity.x = Mathf.MoveTowards(Velocity.x, 0, deAcceleration * Time.deltaTime);
        }
        // check wall
        if (Velocity.x > 0 && rightRay || Velocity.x < 0 && leftRay)
        {
            Velocity.x = 0;
        }
    }

    private void CharacterMove()
    {
        transform.position += Velocity * Time.deltaTime;
    }

    private void CalculateJumpApex()
    {
        if (!isGround)
        {
            apexPoint = Mathf.InverseLerp(jumpApexThreshold, 0, Mathf.Abs(Velocity.y));
            fallGravity = Mathf.Lerp(minFallGravity, maxFallGraviyt, apexPoint);
        }
        else
        {
            apexPoint = 0;
        }
    }

    private void Jump()
    {
        if (jumpInputDown && coyoteJumpTimer>0)
        {
            Velocity.y = jumpHight;
            jumpInputBufferTimer = 0;
        }
        if (!downRay && jumpInputUp && Velocity.y > 0)
        {
            isJumpEarlyUp = true;
        }
    }

    private void Gravity()
    {
        //float fallSpeed = 0;

        if (isGround)
        {
            if (Velocity.y < 0 && downInfo.collider.tag == "Ground")
            { 
                Velocity.y = 0;
                transform.position = downInfo.point+ new Vector3(0,0.5f,0);
                Debug.Log("self: " + transform.position);
                Debug.Log("Hit point: " + downInfo.point);
                isJumpEarlyUp = false;
            }
            return;
        }
        else
        {
            float fallspeed;
            if (isJumpEarlyUp && Velocity.y > 0)
            {
                fallspeed = fallGravity * jumpEarlyMul;
            }
            else
            {
                fallspeed = fallGravity;
            }
            // 可加判断，是否为提前松开jump
            Velocity.y -= fallspeed * Time.deltaTime;
        }

        if (Velocity.y < gravityClamp)
        {
            Velocity.y = gravityClamp;
        }

    }

    private void JumpOptimation()
    {
        // coyote Jump
        if (downRay)
        {
            coyoteJumpTimer = coyoteJump;
        }
        else
        {
            coyoteJumpTimer -= Time.deltaTime;
        }

        //Inputbuffer
        if (jumpInputDown)
        {
            jumpInputBuffer = jumpInputBufferTimer;
        }
        else
        {
            jumpInputBufferTimer -= Time.deltaTime;
            jumpInputBufferTimer = Mathf.Clamp(jumpInputBufferTimer, 0, jumpInputBuffer);
        }
    }

    public void InputDetector()
    {
        moveDir = PlayerInput._instance.moveDir;
        jumpInputDown = PlayerInput._instance.jumpBtnDown;
        jumpInputUp = PlayerInput._instance.jumpBtnUp;
    }


    private void RayDetector()
    {
        rightRay = Physics.Raycast(transform.position, Vector3.right, out rightInfo, rayDis);
        leftRay = Physics.Raycast(transform.position, Vector3.left, out leftInfo, rayDis);
        upRay = Physics.Raycast(transform.position, Vector3.up, out upInfo, rayDis);
        downRay = Physics.Raycast(transform.position, Vector3.down, out downInfo, rayDis);
        Debug.DrawLine(transform.position, transform.position + Vector3.down * rayDis, Color.red, 1);
    }

    private bool GroundCheck()
    {
        bool grounded = downRay;
        return grounded;
    }
}


