using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SpiderController : MonoBehaviour
{
    public float speed = 1f;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (Mathf.Abs(rb.velocity.x) < speed)
        {
            if (Input.GetKey(KeyCode.D))
            {
                rb.AddForce(Vector2.right * 50f);
            }
            if (Input.GetKey(KeyCode.A))
            {
                rb.AddForce(Vector2.right * -50f);
            }
        }
    }
}
