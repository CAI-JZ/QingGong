using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour
{
    [SerializeField] GameObject[] Winds;
    [SerializeField] private List<GameObject> currentWinds = new List<GameObject>();

    private void Start()
    {
        
    }

    private void Update()
    {
        if (currentWinds.Count > 0)
        {
            return; 
        }
        //float duration = Random.Range(0.2f, 2f);
        //Invoke("RandomWind", duration);
        RandomWind();
    }

    private void RandomWind()
    {
        int count = Random.Range(1, Winds.Length-1);
        for (int i = 0; i < count; i++)
        {
            int w = Random.Range(0, Winds.Length - 1);
            if (currentWinds.Contains(Winds[w]))
            {
                w = w >= Winds.Length - 1 ? w - 1 : w + 1;
            }
            Winds[w].SetActive(true);
            currentWinds.Add(Winds[w]);
            
        }
        Invoke("WindDisappear", 10f);
    }

    private void WindDisappear()
    {
        foreach (GameObject w in currentWinds)
        {
            w.SetActive(false);
        }
        currentWinds.Clear();
    }
}
