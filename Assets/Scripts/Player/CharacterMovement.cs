using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class CharacterMovement : MonoBehaviour
{
    //Reference
    PlayerController _controller;

    [Header("RAY")]
    //RayCast
    [SerializeField] private float rayDis = 1f;
    [SerializeField] private float rayDisDown = 1f;
    [SerializeField ]public RaycastHit2D rightHit, leftHit, upHit, Grounded;

    public Vector3 wallForward;
    public float wallAngle;
    public bool canSlpoeWalk;

    [Header("QI")]
    //qinggong
    [SerializeField] public float qiJumpPower = 40;
    private bool isQi;
    private bool useQi;
    [SerializeField] private float qiGraviytMul;
    private QiValue _qiValue;
    [SerializeField] private bool canRecharge;
    [SerializeField] private BorrowableType currentRechargeType = BorrowableType.Defult;
    [SerializeField] private float rechargeTime;
    private float rechargeTimer;


    private void Awake()
    {
        _controller = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        RayDetector();
        //WallWalk();
        Physics.SyncTransforms();
    }

   

    //private void WallWalk()
    //{
    //    if (Grounded && Grounded.collider.tag == "Slope")
    //    {
    //        Vector3 normal = Grounded.normal;
    //        Vector3 forward = (Vector3.Cross(normal, Vector3.forward)).normalized;
    //        //Debug.DrawLine(transform.position, transform.position + forward, Color.green);
    //        //Debug.DrawLine(transform.position, transform.position + Vector3.right, Color.blue);
    //        //Debug.DrawLine(transform.position, transform.position + Vector3.forward * 2, Color.black);
    //        //Debug.DrawLine(transform.position, transform.position + normal * 2, Color.red);
    //        float angle = Vector3.Dot(forward, Vector3.right);
    //        wallAngle = angle;
    //        wallForward = forward;
    //        //if (angle >= -0.01f)
    //        //{ 
    //        //    _controller.velocity = new Vector3 (wallForward.x, wallForward.y, _controller.velocity.z) * 10 * _controller.InputDir;
    //        //}
    //    }
    //    else
    //    {
    //        wallAngle = 0;
    //    }
    //}

    private void RayDetector()
    {

        rightHit = Physics2D.Raycast(transform.position, Vector2.right, rayDis, (1 << 10 | 1 << 6));
        leftHit = Physics2D.Raycast(transform.position, Vector2.left, rayDis, (1 << 10 | 1 << 6));
        upHit = Physics2D.Raycast(transform.position, Vector2.up, rayDis, (1 << 10));
        Grounded = Physics2D.Raycast(transform.position, Vector2.down, rayDisDown, (1 << 10 | 1 << 6));

        //Debug.DrawLine(transform.position, transform.position + Vector3.down * rayDisDown, Color.red, 1);
        Debug.DrawLine(transform.position, transform.position + Vector3.right * rayDis, Color.red, 1);
    }


    //private void UseQinggong()
    //{
    //    if (!downRay && useQi && _qiValue.DecreaseQi(1))
    //    {
    //        StopCoroutine(Qinggong());
    //        StartCoroutine(Qinggong());
    //        //Velocity.y = qiJumpPower;
    //        //if (moveDir < 0)
    //        //{
    //        //    Velocity.x = 60f * moveDir;
    //        //}
    //        //else
    //        //{
    //        //    Velocity.x = 60f;
    //        //}

    //        // Velocity.x = Mathf.Lerp(Velocity.sx, 0, 0.2f * Time.deltaTime);
    //    }
    //}

    //待优化
    //IEnumerator Qinggong()
    //{
    //    float inputHori = moveDir;
    //    float inputVert = 1;
    //    Vector2 dashDir = new Vector2(moveDir, inputVert).normalized;
    //    isControllable = false;
    //    int i = 0;
    //    while (i < 9)
    //    {
    //        Velocity = dashDir * qiJumpPower;
    //        i++;
    //        yield return new WaitForFixedUpdate();
    //    }
    //    Velocity = Vector3.zero;
    //    isControllable = true;
    //}

    //private void BorrowPower()
    //{
    //    if (CheckRechargeableItem())
    //    {
    //        Debug.Log("可以借力");
    //        _qiValue.IncreaseQi(1);
    //        StopCoroutine("Qinggong");
    //        StartCoroutine("Qinggong");
    //        }
    //    else
    //    {
    //        Debug.Log("不可以借力");
    //    }
    //}


    //移动到QinggongScript
    //private bool CheckRechargeableItem()
    //{
    //    if (downInfo.collider != null && downInfo.collider.tag != "Ground")
    //    {
    //        downInfo.collider.gameObject.TryGetComponent(out IBorrow borrowable);
    //        GetItemInfo(borrowable);
    //    }
    //    else if (rightInfo.collider != null && rightInfo.collider.tag != "Ground")
    //    {
    //        rightInfo.collider.gameObject.TryGetComponent(out IBorrow borrowable);
    //        GetItemInfo(borrowable);
    //    }
    //    else if (leftInfo.collider != null && leftInfo.collider.tag != "Ground")
    //    {
    //        leftInfo.collider.gameObject.TryGetComponent(out IBorrow borrowable);
    //        GetItemInfo(borrowable);
    //    }

    //    return canRecharge;
    //}

    //private void GetItemInfo (IBorrow bor)
    //{
    //    currentRechargeType = bor.GetBorrowableType();
    //    StopCoroutine(CanRechargeWindow());
    //    StartCoroutine(CanRechargeWindow());
    //}

    //IEnumerator CanRechargeWindow()
    //{
    //    canRecharge = true;
    //    rechargeTimer = rechargeTime;
    //    while (rechargeTimer > 0)
    //    {
    //        rechargeTimer -= Time.deltaTime;
    //        yield return new WaitForFixedUpdate();
    //    }
    //    rechargeTimer = 0f;
    //    canRecharge = false;
    //    currentRechargeType = BorrowableType.Defult;
    //}
}


