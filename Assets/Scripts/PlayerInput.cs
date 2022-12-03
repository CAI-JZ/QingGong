using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput _instance { get; private set; }

    //PlayerInput
    private float _horizontalInput;
    private bool _jumpInputDown;
    private bool _jumpInputUp;
    public float moveDir => _horizontalInput;
    public bool jumpBtnDown => _jumpInputDown;
    public bool jumpBtnUp => _jumpInputUp;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _jumpInputDown = Input.GetButtonDown("Jump");
        _jumpInputUp = Input.GetButtonUp("Jump");
    }
}
