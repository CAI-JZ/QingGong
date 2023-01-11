using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    private GameObject player;
    private Vector3 targetPosition;
    [SerializeField]
    private Vector3 cameraOffset;
    [SerializeField]
    private float cameraDamp;


    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void LateUpdate()
    {
        targetPosition = player.transform.position + cameraOffset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, cameraDamp);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
