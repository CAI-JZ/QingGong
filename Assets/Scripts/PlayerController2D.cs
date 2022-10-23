using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    public enum PLAYERSTATE
    {
        Qi,
        Normal
    }

    private float AxisX;
    private float MoveSpeed = 200;

    private bool IsJump;
    private bool IsGrounded;
    private bool IsQi;
    private float FallMutiplier = 2.5f;
    [Range(5,15)]
    [SerializeField]private float JumpVelocity =7;
    private float canJump = 0.2f;
    private float canJumpTimer;

    private float JumpBuffer = 0.2f;
    private float JumpBufferTimer;

    private float QiValue = 10;

    [SerializeField]private Transform GroundCheck;
    [SerializeField]private LayerMask GroundLayer;

    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        AxisX = Input.GetAxis("Horizontal");
        IsJump = Input.GetButtonDown("Jump");
        IsGrounded = Physics2D.OverlapCircle(GroundCheck.position, 0.2f, GroundLayer);
        IsQi = Input.GetKey(KeyCode.Q);


        if (IsQi)
        {
            Debug.Log("开始用");
            UseQi();
        }
        else
        { 
            MoveSpeed = 200;
        }

        //Coyote Jump
        if (IsGrounded)
        {
            canJumpTimer = canJump;
        }
        else
        {
            canJumpTimer -= Time.deltaTime;

        }

        // Input Buffer - Jump
        if (IsJump)
        {
            JumpBufferTimer = JumpBuffer;
           
        }
        else
        {
            JumpBufferTimer -= Time.deltaTime;
            if (JumpBufferTimer < 0)
            {
                JumpBufferTimer = 0;
            }
        }

        Jump();

    }

    private void FixedUpdate()
    {
        Move(AxisX);
        
    }

    void Move(float inputX) 
    {
        _rigidbody.velocity = new Vector2(inputX * MoveSpeed * Time.deltaTime , _rigidbody.velocity.y);
    }

    void Jump()
    {
        // Can Jump
        if (JumpBufferTimer > 0f && canJumpTimer > 0f)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, JumpVelocity);
            JumpBufferTimer = 0;
        }

        if (Input.GetButtonUp("Jump") && _rigidbody.velocity.y > 0)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y * 0.5f);

            canJumpTimer = 0;
        }
         //Fall more faster , long press get jump higher
        if (_rigidbody.velocity.y < 0)
        {
            _rigidbody.velocity += Vector2.up * Physics2D.gravity.y * (FallMutiplier - 1) * Time.deltaTime;
        }
   
    }

    void UseQi()
    {
        Debug.Log("发力");
        //MoveSpeed = 600;
        //Vector2 force = new Vector2(5,3);
        _rigidbody.AddForce(Vector2.right * 400);
    }
}
