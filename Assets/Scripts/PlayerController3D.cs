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

    //Jump
    private bool IsJump;
    private bool IsGrounded;
    private bool IsQi;
    private float FallMutiplier = 3f;
    //[Range(5, 15)]
    [SerializeField] private float JumpVelocity = 7;

    private float coyoteJump = 0.2f;
    private float coyoteJumpTimer;
    private float JumpBuffer = 0.2f;
    private float JumpBufferTimer;
    private bool CanDash;

    private float QiValue = 10;
    [SerializeField]private float qiMoveMul = 1;
    [SerializeField]private float qiJumpMul = 1;
    private float QiMoveMul = 1.5f;
    private float QiJumpMul = 1.2f;

    [SerializeField] private Transform GroundCheck;
    [SerializeField] private LayerMask GroundLayer;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
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
            if (playerState == PLAYERSTATE.Qi)
            {
                CanDash = true;
            }
        }
        else
        {
            //松开按键后开始计时
            JumpBufferTimer -= Time.deltaTime;
            if (JumpBufferTimer < 0)
            {
                JumpBufferTimer = 0;
            }
            CanDash = false;
        }
    }

    private void FixedUpdate()
    {
        Move(AxisX);
        Jump();
        UseQi();
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
        if (CanDash && !IsGrounded)
        {
            Vector3 dir = (Vector3.right * 2 + Vector3.up * 3).normalized;
            _rigidbody.AddRelativeForce(dir* dashpower, ForceMode.Acceleration);
            Debug.Log("Qinggong");
        }

    }

    public float dashpower;

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
}


