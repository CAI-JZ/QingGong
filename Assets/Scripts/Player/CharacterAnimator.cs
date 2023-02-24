using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private SpriteRenderer _character;
    private PlayerController _controller;

    [Header("Input")]
    private float inputHorizontal;

    private void Awake()
    {
        _character = GetComponentInChildren<SpriteRenderer>();
        _controller = GetComponentInParent<PlayerController>();
    }

    private void Update()
    {
        FlipSprite();
    }

    private void FlipSprite()
    {
        if (_character == null) return;
        if (_controller.Velocity.x < 0)
        {
            _character.flipX = true;
        }
        else if (_controller.Velocity.x > 0)
        {
            _character.flipX = false;
        }
    }


}
