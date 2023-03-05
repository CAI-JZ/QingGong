using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBranch : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        { 
            //collision.gameObject.TryGetComponent(out IBorrow borrowable)
        }
    }
}
