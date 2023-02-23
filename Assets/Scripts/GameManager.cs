using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]public Rect Tutorial;

    // Start is called before the first frame update
    void Start()
    {
        
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
