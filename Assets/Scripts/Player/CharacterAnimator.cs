using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private SpriteRenderer _character;
    private PlayerController _controller;
    private MovementController controller;

    [Header("Input")]
    private float inputHorizontal;

    private void Awake()
    {
        _character = GetComponentInChildren<SpriteRenderer>();
        _controller = GetComponentInParent<PlayerController>();
        controller = GetComponentInParent<MovementController>();
    }

    private void Update()
    {
        FlipSprite();
    }

    private void FlipSprite()
    {
        if (_character == null) return;
        _character.flipX = controller.playerDir == CharDir.Right ? false : true;
    }

}
