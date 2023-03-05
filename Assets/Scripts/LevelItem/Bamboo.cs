using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bamboo : MonoBehaviour
{
    [SerializeField] Rigidbody2D bamboo;
    [SerializeField] bool addForce;
    [SerializeField] float ForcePower;
    [SerializeField] bool hasAdded;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (addForce)
        {
            AddForce();
        }
    }

    private void AddForce()
    {
        //hasAdded = true;
        bamboo.velocity = Vector2.left * ForcePower;
    }
}
