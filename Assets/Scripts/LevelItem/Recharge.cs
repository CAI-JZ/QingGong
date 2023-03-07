using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recharge : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("ø…“‘ª÷∏¥");

            var move = collision.gameObject.GetComponent<MovementController>();
            move.RechargeWindow();
        }
    }

    private void EventOnRecharge()
    { 
    
    }
}
