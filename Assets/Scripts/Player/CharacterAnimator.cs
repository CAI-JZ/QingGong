using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private SpriteRenderer _character;

    [Header("Input")]
    private float inputHorizontal;

    private void Awake()
    {
        _character = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        InputDetactor();
        FlipSprite();
    }

    private void FlipSprite()
    {
        if (_character == null) return;
        if (inputHorizontal < 0)
        {
            _character.flipX = true;
        }
        else if (inputHorizontal > 0)
        {
            _character.flipX = false;
        }
    }

    private void InputDetactor()
    {
        inputHorizontal = PlayerInput._instance.HorizontalInput; 
    }
}
