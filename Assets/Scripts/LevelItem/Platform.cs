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
        Debug.Log("我应该在进入前出现");
        base.OnTriggerEnter(other);
        Debug.Log("进入了，确信");
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        Collider target = collision.collider;
        if (target.tag == "Player")
        {
            //调用game manager让游戏结束，让角色回到某个初始位置
            Debug.Log("Game over, character die!");

        }
    }

}
