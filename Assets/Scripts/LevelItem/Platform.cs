using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform :BorrowableBase
{

    private void Awake()
    {
        
    }

    protected override void OnTriggerEnter(Collider other)
    {
        Debug.Log("��Ӧ���ڽ���ǰ����");
        base.OnTriggerEnter(other);
        Debug.Log("�����ˣ�ȷ��");
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        Collider target = collision.collider;
        if (target.tag == "Player")
        {
            //����game manager����Ϸ�������ý�ɫ�ص�ĳ����ʼλ��
            Debug.Log("Game over, character die!");

        }
    }

}
