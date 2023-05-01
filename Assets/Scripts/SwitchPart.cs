using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPart : MonoBehaviour
{
    
    public int partNum;

    [SerializeField] AudioSource switchAudio;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            switchAudio.Play();
            float t = switchAudio.clip.length;
            Debug.Log(t);
            UIManager.instance.SwitchTitle(partNum);
            Invoke("SetUnactive", t);
        }
    }

    private void SetUnactive()
    {
        gameObject.SetActive(false);
    }
}
