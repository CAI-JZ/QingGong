using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    //Move
    [Header("BASIC DATA")]
    [SerializeField] Vector3 velocity;
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
    private float apexPoint;

    [Header("GRAVITY")]
    [SerializeField] private float gravityClamp = -30f;
    [SerializeField] private float minFallGravity = 80f;
    [SerializeField] private float maxFallGraviyt = 120f;
    private bool isJumpEarlyUp;
    [SerializeField] public float fallGravity;

    //Moves
    [Header("MOVE")]
    [SerializeField] float moveDir;
    public float moveAcceleration = 50f;
    public float deAcceleration = 50f;
    public float moveClamp = 13f;

    [Header("RAY")]
    //RayCast
    [SerializeField] private float rayDis = 0.5f;
    [SerializeField] private float rayDisDown = 1f;
    [SerializeField] public bool rightRay, leftRay, upRay, downRay;
    public RaycastHit rightInfo, leftInfo, upInfo, downInfo;

    private void Awake()
    {
        isControllable = true;
    }

    // Update is called once per frame
    private void Update()
    {
        RayDetector();
        InputDetector();
        JumpOptimation();
        CalculateWalk();
        CalculateJumpApex();
        Gravity();
        Jump();
        WallWalk();
        CharacterMove();

        //physics.SyncTransforms();
    }

    private void FixedUpdate()
    {

    }

    private void CharacterMove()
    {
        transform.position += velocity * Time.deltaTime;
    }

    private void CalculateWalk()
    {
        if (isControllable)
        {
            if (moveDir != 0)
            {
                // speed acceleration when input
                velocity.x += moveDir * moveAcceleration * Time.deltaTime;
                velocity.x = Mathf.Clamp(velocity.x, -moveClamp, moveClamp);
            }
            else
            {
                //deacceleration  when not input
                velocity.x = Mathf.MoveTowards(velocity.x, 0, deAcceleration * Time.deltaTime);
            }
            //check wall
            if (velocity.x > 0 && rightRay || velocity.x < 0 && leftRay)
            {
                velocity.x = 0;
            }
        }
    }

    private void CalculateJumpApex()
    {
        if (!downRay)
        {
            apexPoint = Mathf.InverseLerp(jumpApexThreshold, 0, Mathf.Abs(velocity.y));
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
                velocity.y = jumpHight;
                jumpInputBufferTimer = 0;
            }
            if (!downRay && jumpInputUp && velocity.y > 0)
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
            if (velocity.y < 0 && downInfo.collider.tag == "Ground")
            {
                velocity.y = 0;
                transform.position = downInfo.point + new Vector3(0, 0.9f, 0);
                isJumpEarlyUp = false;
            }
            return;
        }
        else
        {
            float fallspeed;
            if (isJumpEarlyUp && velocity.y > 0)
            {
                fallspeed = fallGravity * jumpEarlyMul;
            }
            else
            {
                fallspeed = fallGravity;
            }
            //if (isQi && Velocity.y < 0)
            //{
            //    fallspeed = fallGravity * qiGraviytMul;
            //}
            velocity.y -= fallspeed * Time.deltaTime;
            if (velocity.y < gravityClamp) velocity.y = gravityClamp;

        }
    }
    private void WallWalk()
    {
        if (rightRay || leftRay)
        {

            Vector3 wallNormal = rightRay ? rightInfo.normal : leftInfo.normal;
            Vector3 wallForward = (Vector3.Cross(wallNormal, Vector3.forward)).normalized;
            Debug.DrawLine(transform.position, transform.position + wallForward, Color.green);
            Debug.DrawLine(transform.position, transform.position + Vector3.right, Color.blue);
            Debug.DrawLine(transform.position, transform.position + Vector3.forward * 2, Color.black);
            Debug.DrawLine(transform.position, transform.position + wallNormal * 2, Color.red);
            float angle = Vector3.Dot(wallForward, Vector3.right);
            Debug.Log(angle);
            if (angle >= -0.01f)
            {
                velocity = new Vector3(wallForward.x, wallForward.y, velocity.z) * 10 * moveDir;
            }
        }
    }


    public void InputDetector()
    {
        moveDir = PlayerInput._instance.HorizontalInput;
        jumpInputDown = PlayerInput._instance.jumpBtnDown;
        jumpInputUp = PlayerInput._instance.jumpBtnUp;
        //isQi = Input.GetKey(KeyCode.Mouse0);
        //useQi = Input.GetKeyDown(KeyCode.Mouse0);
    }

    private void RayDetector()
    {
        rightRay = Physics.Raycast(transform.position, Vector3.right, out rightInfo, rayDis, (1 << 6));
        leftRay = Physics.Raycast(transform.position, Vector3.left, out leftInfo, rayDis, (1 << 6));
        upRay = Physics.Raycast(transform.position, Vector3.up, out upInfo, rayDis, (1 << 10));
        downRay = Physics.Raycast(transform.position, Vector3.down, out downInfo, rayDisDown, (1 << 10 | 1 << 6));
        Debug.DrawLine(transform.position, transform.position + Vector3.down * rayDisDown, Color.red, 1);
        Debug.DrawLine(transform.position, transform.position + Vector3.left * rayDis, Color.red, 1);
    }
}
