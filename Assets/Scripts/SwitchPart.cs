using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPart : MonoBehaviour
{
    
    public int partNum;

    [SerializeField] AudioSource switchAudio;

    [SerializeField] private BoxCollider2D trigger;

    private void Start()
    {
        trigger = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            switchAudio.Play();
            float t = switchAudio.clip.length;
            UIManager.instance.SwitchTitle(partNum);
            trigger.enabled = false;
            Invoke("SetUnactive", t);
        }
    }

    private void SetUnactive()
    {
        gameObject.SetActive(false);
    }
}
