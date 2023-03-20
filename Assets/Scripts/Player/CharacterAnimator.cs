using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private MovementController controller;
    [SerializeField] private Animator _animator;

    bool isRun;
    bool isAir;


    private void Awake()
    {
        controller = GetComponentInParent<MovementController>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        isRun = PlayerInput._instance.HorizontalInput != 0;
        isAir = !controller.Grounded;

        Debug.Log(isAir);

        _animator.SetBool("isRun", isRun);
        _animator.SetBool("isAir", isAir);
        _animator.SetBool("isDown", controller.Velocity.y < 0);

        if (controller.IsJumping)
        {
            _animator.SetTrigger("Jump");
        }

        if (controller.Grounded)
        {
            _animator.ResetTrigger("Jump");
        }

    }


}
