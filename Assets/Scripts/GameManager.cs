using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]public Rect Tutorial;
    [SerializeField] public AudioSource background;


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        background.Play();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        string content = "W,A,S,D ÒÆ¶¯";
        GUI.Label(Tutorial, "W,A,S,D ÒÆ¶¯");
    }
}
