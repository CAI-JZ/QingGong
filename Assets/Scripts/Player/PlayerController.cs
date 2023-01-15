using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController _instance {get ; private set;}




    private void Awake()
    {
        _instance = this;
    }


}
