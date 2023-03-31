using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPart : MonoBehaviour
{
    public int partNum;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            UIManager.instance.SwitchTitle(partNum);
            Destroy(gameObject);
        }
    }
}
