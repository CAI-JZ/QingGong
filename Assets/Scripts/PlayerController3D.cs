using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController3D : MonoBehaviour
{
    public enum PLAYERSTATE
    {
        Qi,
        Normal
    }

    [SerializeField]private PLAYERSTATE playerState = PLAYERSTATE.Normal;

    //Move
    private float AxisX;
    [SerializeField] private float MoveSpeed = 200;
    
    private bool IsJump;
    private bool IsGrounded;
    private bool IsQi;
    private float FallMutiplier = 3f;

    //Jump
    //[Range(5, 15)]
    [Header("Jump")]
    [SerializeField] private float JumpVelocity = 7;
    private float coyoteJump = 0.2f;
    private float coyoteJumpTimer;
    private float JumpBuffer = 0.2f;
    private float JumpBufferTimer;

    //QI
    [Header("Qi")]
    private float QiValue = 10;
    [SerializeField]private float qiMoveMul = 1;
    [SerializeField]private float qiJumpMul = 1;
    private float QiMoveMul = 1.5f;
    private float QiJumpMul = 1.2f;

    //Climb wall
    [Header("ClimbWall")]
    [SerializeField]private float climbVelocity = 4f;

    [SerializeField] private Transform GroundCheck;
    [SerializeField] private LayerMask GroundLayer;

    private Rigidbody _rigidbody;

    private bool rightRay, leftRay, upRay, downRay;
    private RaycastHit rightInfo, leftInfo, upInfo, downInfo;
    [SerializeField]private float rayDis = 1f;
    

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        RayDetector();
        AxisX = Input.GetAxis("Horizontal");
        IsJump = Input.GetButtonDown("Jump");
        IsGrounded = Physics.CheckSphere(GroundCheck.position, 0.2f, GroundLayer);
        IsQi = Input.GetButton("Qi");

        if (IsQi)
        {
            playerState = PLAYERSTATE.Qi;
        }
        else
        {
            playerState = PLAYERSTATE.Normal;
        }

        //Coyote Jump
        if (IsGrounded)
        {
            coyoteJumpTimer = coyoteJump;
        }
        else
        {
            coyoteJumpTimer -= Time.deltaTime;
        }

        // Input Buffer - Jump
        if (IsJump)
        {
            JumpBufferTimer = JumpBuffer;
        }
        else
        {
            //松开按键后开始计时
            JumpBufferTimer -= Time.deltaTime;
            if (JumpBufferTimer < 0)
            {
                JumpBufferTimer = 0;
            }      
        }
    }

    private void FixedUpdate()
    {
        Move(AxisX);
        Jump();
        UseQi();
        WalkOnlWall(AxisX);
    }

    private void RayDetector()
    {
        rightRay = Physics.Raycast(transform.position, Vector3.right,out rightInfo, rayDis);
        leftRay = Physics.Raycast(transform.position, Vector3.left, out leftInfo, rayDis);
        upRay = Physics.Raycast(transform.position, Vector3.up, out upInfo, rayDis);
        downRay = Physics.Raycast(transform.position, Vector3.down, out downInfo, rayDis);
        Debug.DrawRay(transform.position, Vector3.right * rayDis, Color.red);
    }

    void Move(float inputX)
    {
        _rigidbody.velocity = new Vector3(inputX * MoveSpeed * qiMoveMul * Time.deltaTime, _rigidbody.velocity.y);
    }

    [Range(0,5)]
    public float jumpMul;
    void Jump()
    {
        // Can Jump
        if (JumpBufferTimer > 0f && coyoteJumpTimer > 0f)
        {

            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, JumpVelocity * Time.deltaTime * qiJumpMul, _rigidbody.velocity.z); 
            JumpBufferTimer = 0;
        }
        if (Input.GetButtonUp("Jump") && _rigidbody.velocity.y > 0)
        {
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _rigidbody.velocity.y * 0.5f * qiJumpMul, _rigidbody.velocity.z);
            coyoteJumpTimer = 0;
        }
        //Fall more faster , long press get jump higher
        if (_rigidbody.velocity.y < 0)
        {
            if (playerState == PLAYERSTATE.Normal)
            {
                _rigidbody.velocity += Vector3.up * Physics.gravity.y * (FallMutiplier - 1) * Time.deltaTime;
            }
            else if (playerState == PLAYERSTATE.Qi)
            {
                //减缓下落速度
                _rigidbody.velocity += Vector3.up * Physics.gravity.y * (0.1f) * Time.deltaTime;
            }
        }
    }

    void UseQi()
    {
        if (IsQi)
        {  
            qiJumpMul = QiJumpMul;
            qiMoveMul = QiMoveMul;    
        }
        else
        {
            qiJumpMul = 1;
            qiMoveMul = 1;
            
        }
        
    }

    private void WalkOnlWall(float inputX)
    {
        if (playerState == PLAYERSTATE.Qi && inputX != 0)
        { 
            if (leftInfo.collider != null && leftInfo.collider.tag == "Climbable")
            {
                _rigidbody.velocity = new Vector3(0, climbVelocity * Time.deltaTime, _rigidbody.velocity.z);
                Debug.Log("阴暗的左边爬行");
            }
            else if (rightInfo.collider != null && rightInfo.collider.tag == "Climbable")
            {
                _rigidbody.velocity = new Vector3(0, climbVelocity * Time.deltaTime, _rigidbody.velocity.z);
                Debug.Log("阴暗的右边爬行");
            }
        
        }
    }

    private void JieLi()
    { 
    
    }
}


