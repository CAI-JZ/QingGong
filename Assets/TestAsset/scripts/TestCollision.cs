using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("any enter");
        if (collision.collider.tag == "Player")
        {
            Debug.Log("player enter");
        }
    }

}
