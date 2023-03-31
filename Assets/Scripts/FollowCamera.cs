using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameObject player;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private Vector2 cameraOffset;
    [SerializeField] private Vector2 focusAreaSize;

    [Header("Horizontal Control")]
    private float lookAheadDirx;
    private float targetLookAheadX;
    private float currentLookAheadX;
    private bool lookAheadStoped;
    [SerializeField] private float lookAheadDisX;
    [SerializeField] private float dampVelocityX;
    [SerializeField] private float dampTimeX;

    [Header("Vertical Control")]
    [SerializeField] private float dampVelocityY;
    [SerializeField] private float dampTimeY;

    [Header("WhenPlayerRespawn")]
    [SerializeField] private Vector3 dampVelocity;
    [SerializeField] private float dampTime;

    private bool isStart;
    public bool isFollow;
    public bool isMoving;
    public Vector3 checkPoint;
    private FocusArea focusArea;

    private void Awake()
    {
        isStart = false;
        isFollow = false;
    }

    public void GameStart()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerCollider = player.GetComponent<Collider2D>();
        focusArea = new FocusArea(playerCollider.bounds, focusAreaSize);
        isStart = true;
        isFollow = true;
    }

    private void LateUpdate()
    {

        if (!isStart || !isFollow)
        {
            return;
        }

        // Update focus area;
        focusArea.Update(playerCollider.bounds);

        Vector2 focusPosition = focusArea.center + cameraOffset;

        // horizontal control
        if (focusArea.velocity.x != 0)
        {
            lookAheadDirx = Mathf.Sign(focusArea.velocity.x);
            if (PlayerInput._instance.HorizontalInput != 0 && Mathf.Sign(PlayerInput._instance.HorizontalInput) == lookAheadDirx)
            {
                lookAheadStoped = false;
                targetLookAheadX = lookAheadDirx * lookAheadDisX;
            }
            else if(!lookAheadStoped)
            {
                targetLookAheadX = currentLookAheadX + (lookAheadDirx * lookAheadDisX - currentLookAheadX) / 1.5f;
                lookAheadStoped = true;
            }
        }
        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref dampVelocityX, dampTimeX);
        focusPosition += Vector2.right * currentLookAheadX;

        // vertical control;
        focusPosition.y = Mathf.SmoothDamp(transform.position.y,focusPosition.y, ref dampVelocityY, dampTimeY);

        // Camera Smooth Follow
        transform.position = new Vector3(focusPosition.x, focusPosition.y, -10);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.4f);
        Gizmos.DrawCube(focusArea.center, focusAreaSize);
        Gizmos.DrawSphere(focusArea.center, 0.2f);
    }

}

public struct FocusArea
{
    public Vector2 center;
    float l, r, t, b;
    public Vector2 velocity;

    public FocusArea(Bounds targetBounds, Vector2 focusAreaSize)
    {
        l = targetBounds.center.x - focusAreaSize.x / 2;
        r = targetBounds.center.x + focusAreaSize.x / 2;
        t = targetBounds.min.y + focusAreaSize.y;
        b = targetBounds.min.y;

        center = new Vector2((l + r) / 2, (t + b) / 2);
        velocity = Vector2.zero;
    }

    public void Update(Bounds targetBounds)
    {
        float shiftX = 0;
        if (targetBounds.min.x < l)
        {
            shiftX = targetBounds.min.x - l;
        }
        else if (targetBounds.max.x > r)
        {
            shiftX = targetBounds.max.x - r;
        }

        l += shiftX;
        r += shiftX;

        float shiftY = 0;
        if (targetBounds.min.y < b)
        {
            shiftY = targetBounds.min.y - b;
        }
        else if (targetBounds.max.y > t)
        { 
            shiftY = targetBounds.max.y - t;
        }

        t += shiftY;
        b += shiftY;

        center = new Vector2((l + r) / 2, (t + b) / 2);
        velocity = new Vector2(shiftX, shiftY);
    }
}
