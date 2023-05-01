using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinPoint : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            int treasure = GameManager.instance.GetCollectiong();
            ScoreSave.instance.UpdateTreasureCount(treasure);
            SceneManager.LoadScene(1);
        }
    }
}
