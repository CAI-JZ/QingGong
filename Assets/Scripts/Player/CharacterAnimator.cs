using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [Header("Reference")]
    private SpriteRenderer _character;
    private PlayerController _controller;
    [SerializeField] private MovementController controller;
    [SerializeField] private Animator _animator;

    bool isRun;

    private void Awake()
    {
        controller = GetComponentInParent<MovementController>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        isRun = PlayerInput._instance.HorizontalInput != 0;
        _animator.SetBool("isRun", isRun);
    }


}
