using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Reference")]
    private GameObject player;
    [SerializeField] private Collider2D playerCollider;

    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private Vector2 focusAreaSize;
    [SerializeField]private float offsetY;

    [Header("Horizontal Control")]
    private float lookAheadDirx;
    private float targetLookAheadX;
    [SerializeField] private float lookAheadDisX;
    private float currentLookAheadX;
    [SerializeField]private float dampVelocity;
    [SerializeField] private float dampTime;

    private bool lookAheadStoped;

    private FocusArea focusArea;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        playerCollider = player.GetComponent<Collider2D>();
        focusArea = new FocusArea(playerCollider.bounds, focusAreaSize);
    }

    private void LateUpdate()
    {

        // UPDATE FOCUS AREA 
        focusArea.Update(playerCollider.bounds);

        Vector2 focusPosition = (Vector3)focusArea.center + cameraOffset;

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
                targetLookAheadX = currentLookAheadX + (lookAheadDirx * lookAheadDisX - currentLookAheadX) / 4;
                lookAheadStoped = true;
            }
        }
        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref dampVelocity, dampTime);
        focusPosition += Vector2.right * currentLookAheadX;

        // vertical control;
        //if()

        transform.position = new Vector3(focusPosition.x, player.transform.position.y+ offsetY, -10);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.4f);
        Gizmos.DrawCube(focusArea.center, focusAreaSize);
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
