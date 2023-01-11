using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    //Move
    [Header("BASIC DATA")]
    [SerializeField]
    Vector3 Velocity;
    [SerializeField] float moveDir;
    private bool isControllable;

    [Header("JUMP")]
    //Character Basic data
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
    [SerializeField] public float fallGravity;

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


    [Header("QI")]
    //qinggong
    [SerializeField] public float qiJumpPower = 10;
    private float qiValue = 100;
    private bool isQi;
    private bool useQi;
    [SerializeField] private float qiGraviytMul;

    [Header("OTHER")]
    //GroundCheck
    [SerializeField]
    private LayerMask groundLayer;
    //[SerializeField] private bool isGround;

    private void Awake()
    {
        isControllable = true;
    }

    // Update is called once per frame
    void Update()
    {
        RayDetector();
        InputDetector();

        JumpOptimation();
        CalculateWalk();
        CalculateJumpApex();
        Gravity();
        Jump();
        UseQinggong();
        CharacterMove();

    }

    private void CharacterMove()
    {
        transform.position += Velocity * Time.deltaTime;
    }

    private void CalculateWalk()
    {
        if (isControllable)
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
    }

    private void CalculateJumpApex()
    {
        if (!downRay)
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
        if (isControllable)
        {
            if (jumpInputBufferTimer > 0 && coyoteJumpTimer > 0)
            {
                Velocity.y = jumpHight;
                jumpInputBufferTimer = 0;
            }
            if (!downRay && jumpInputUp && Velocity.y > 0)
            {
                isJumpEarlyUp = true;
            }

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
            if (jumpInputBufferTimer < 0) jumpInputBufferTimer = 0;

        }
    }

    private void Gravity()
    {
        //float fallSpeed = 0;

        if (downRay)
        {
            if (Velocity.y < 0 && downInfo.collider.tag == "Ground")
            {
                Velocity.y = 0;
                transform.position = downInfo.point + new Vector3(0, 0.5f, 0);
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
            if (isQi && Velocity.y < 0)
            {
                fallspeed = fallGravity * qiGraviytMul;
            }
            Velocity.y -= fallspeed * Time.deltaTime;
            if (Velocity.y < gravityClamp) Velocity.y = gravityClamp;

        }
    }

    public void InputDetector()
    {
        moveDir = PlayerInput._instance.moveDir;
        jumpInputDown = PlayerInput._instance.jumpBtnDown;
        jumpInputUp = PlayerInput._instance.jumpBtnUp;
        isQi = Input.GetKey(KeyCode.LeftShift);
        useQi = Input.GetKeyDown(KeyCode.LeftShift);
    }

    private void RayDetector()
    {
        rightRay = Physics.Raycast(transform.position, Vector3.right, out rightInfo, rayDis, (1 << 10));
        leftRay = Physics.Raycast(transform.position, Vector3.left, out leftInfo, rayDis, (1 << 10));
        upRay = Physics.Raycast(transform.position, Vector3.up, out upInfo, rayDis, (1 << 10));
        downRay = Physics.Raycast(transform.position, Vector3.down, out downInfo, rayDis, (1 << 10));
        Debug.DrawLine(transform.position, transform.position + Vector3.down * rayDis, Color.red, 1);
    }

    private void UseQinggong()
    {
        if (!downRay && useQi)
        {
            StopCoroutine("Qinggong");
            StartCoroutine("Qinggong");
            //Velocity.y = qiJumpPower;
            //if (moveDir < 0)
            //{
            //    Velocity.x = 60f * moveDir;
            //}
            //else
            //{
            //    Velocity.x = 60f;
            //}

            // Velocity.x = Mathf.Lerp(Velocity.sx, 0, 0.2f * Time.deltaTime);

        }
    }

    //待优化
    IEnumerator Qinggong()
    {
        Debug.Log("携程开始");
        float inputHori = moveDir;
        float inputVert = 1;
        Vector2 dashDir = new Vector2(moveDir, inputVert).normalized;
        isControllable = false;
        int i = 0;
        while (i < 9)
        {
            Velocity = dashDir * 30;
            i++;
            yield return new WaitForFixedUpdate();
        }
        Velocity = Vector3.zero;
        isControllable = true;
        
    }
}


