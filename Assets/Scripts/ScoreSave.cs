using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSave : MonoBehaviour
{
    public static ScoreSave instance { get; private set; }

    private int treasureCount;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        treasureCount = 0;
    }

    public int GetTreasureCount()
    {
        return treasureCount;
    }

    public void UpdateTreasureCount(int newCount)
    {
        treasureCount = newCount;
    }

    public void ResetCount()
    {
        treasureCount = 0;
    }
}
