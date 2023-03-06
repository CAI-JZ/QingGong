using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBranch : MonoBehaviour//,IBorrow
{
    public float swingPower;
    public float deformPower;
    Rigidbody2D rbTree;
    private BorrowableType bTpye = BorrowableType.Tree;

    HingeJoint2D hinge;
    [SerializeField] Transform playerCheck;
    RaycastHit2D hitOut;
    [SerializeField] float rayDis;
    [SerializeField]LayerMask layer;

    private void Awake()
    {
        rbTree = GetComponent<Rigidbody2D>();
        hinge = GetComponent<HingeJoint2D>();
    }

    private void Update()
    {
        if (IsDeform())
        {
            Deform();
        }
        
    }

    private bool IsDeform()
    {
        hitOut = Physics2D.Raycast(playerCheck.position, Vector2.right, rayDis, layer);
        Debug.DrawLine(playerCheck.position, (Vector2)playerCheck.position + Vector2.right * rayDis, Color.blue);
        if (!hitOut)  return false;
        
        if (hitOut.collider.tag == "Player")
        {
            float dis = hitOut.point.x - playerCheck.position.x;
            //hinge.limits.max = dis;
            
            if (dis > 0.4f)
            {
                return true;
            }
            Debug.Log("¼ì²âµ½Player:" + dis);
        }
        return false;
    }

    private void Deform()
    {
        rbTree.velocity = Vector2.down * deformPower;
    }

    public BorrowableType GetBorrowableType()
    {
        return bTpye;
    }

    public void Swing()
    {
        rbTree.AddForce(Vector2.down * swingPower, ForceMode2D.Impulse);
        
    }
}
