using System.Collections;
using System;
using UnityEngine;

public class Platform : MonoBehaviour,IBorrow
{
    [SerializeField] protected int qiRecharge;
    [SerializeField] protected BorrowableType type = BorrowableType.Defult;

    public BorrowableType GetBorrowableType()
    {
        return type;
    }

    public void RechargeQi(Vector3 velocity)
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("砰");
        Collider target = collision.collider;
        if (target.tag == "Player")
        {
            //调用game manager让游戏结束，让角色回到某个初始位置
            Debug.Log("Game over, character die!");

        }
    }

}
