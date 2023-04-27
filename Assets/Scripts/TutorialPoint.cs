using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPoint : MonoBehaviour
{
    public CanvasGroup currentTutorial;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("Show Tutorial");
            UIManager.instance.ShowUI(currentTutorial);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("Hide Tutorial");
            UIManager.instance.HideUI(currentTutorial, true);
        }
    }

}
