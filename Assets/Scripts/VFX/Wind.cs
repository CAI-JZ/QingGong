using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    public ParticleSystem[] Winds;
    [SerializeField] private ParticleSystem currentWind;

    private void OnEnable()
    {
        RandomWindController();
    }

    private void Awake()
    {
        RandomWindController();
    }

    private void RandomWindController()
    {
        int t = Random.Range(0, 2);
        currentWind = Winds[t];
        currentWind.Play();
    }
}
