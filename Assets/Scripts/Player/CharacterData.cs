using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
    //BaseData
    [Header("Base Data")]
    public float moveSpeed =0;

    //Move
    [Header("Move")]
    [SerializeField]private float moveAcceleration = 50f;
    [SerializeField]private float deAcceleration = 50f;
    [SerializeField]private float moveClamp = 13f;

    private void Update()
    {

    }

    public void MoveAcceleraton()
    {
        moveSpeed = +moveAcceleration * Time.deltaTime;
        moveSpeed = Mathf.Clamp(moveSpeed, -moveClamp, moveClamp);
    }

    public void MoveDeacceleration()
    {
        moveSpeed = Mathf.MoveTowards(moveSpeed, 0, deAcceleration * Time.deltaTime);
    }

}
