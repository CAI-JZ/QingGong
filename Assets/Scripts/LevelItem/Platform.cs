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
        Debug.Log("��");
        Collider target = collision.collider;
        if (target.tag == "Player")
        {
            //����game manager����Ϸ�������ý�ɫ�ص�ĳ����ʼλ��
            Debug.Log("Game over, character die!");

        }
    }

}
