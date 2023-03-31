using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput _instance { get; private set; }

    //Input keycode
    [Header("Input Keycode")]
    [SerializeField]public KeyCode jumpKey = KeyCode.Escape;
    //public button
    public KeyCode useQiKey;

    //PlayerInput
    private float _horizontalInput;
    private float _verticalInput;
    private bool _jumpInputDown;
    private bool _jumpInputUp;
    private bool _dash;
    private bool _isPause;

    public float HorizontalInput => _horizontalInput;
    public float VerticalInput => _verticalInput;
    public bool jumpBtnDown => _jumpInputDown;
    public bool jumpBtnUp => _jumpInputUp;
    public bool Dash => _dash;
    public bool Pause => _isPause;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
        //_jumpInputDown = Input.GetKeyDown(jumpKey);
        //_jumpInputUp = Input.GetKeyUp(jumpKey);
        _jumpInputDown = Input.GetButtonDown("Jump");
        _jumpInputUp = Input.GetButtonUp("Jump");
        _dash = Input.GetButtonDown("Dash");
        _isPause = Input.GetKeyDown(KeyCode.Escape);

        if (_dash)
        {
            Debug.Log("Dash");
        }

    }
}
