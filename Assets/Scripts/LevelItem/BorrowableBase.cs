using System.Collections;
using UnityEngine;


public enum BorrowableType
{
    Defult,
    Platform,
    Wall,
    Bamboo,
    Water,
    Grass
}


public abstract class BorrowableBase : MonoBehaviour
{
    protected GameObject player;
    [SerializeField]protected float rechargeTime;
    protected float rechargeTimer;
    [SerializeField] protected int qiRecharge;
    [SerializeField] protected BorrowableType type = BorrowableType.Defult;

    protected void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log("ฝ๘ศ๋มห");
        if (other.tag == "Player")
        {
            StopCoroutine("CanRecharge");
            StartCoroutine("CanRecharge");
        }
    }

    IEnumerator CanRecharge()
    {
        player.GetComponent<CharacterMovement>().UpdateRechargeInfo(true, type);
        rechargeTimer = rechargeTime;
        while (rechargeTimer > 0)
        {
            rechargeTimer -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        rechargeTimer = 0f;
        player.GetComponent<CharacterMovement>().UpdateRechargeInfo(false, BorrowableType.Defult);
    }
}
