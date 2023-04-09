using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGizmo : MonoBehaviour
{
    public Transform constrainPoint;
    public float pointLenth;

    private Vector3 otherPoint;

    private void Start()
    {
        
    }

    private void Update()
    {
        SetPointPos();
    }

    private void SetPointPos()
    {
        if (constrainPoint != null)
        {
            otherPoint = constrainPoint.position + Vector3.up * pointLenth;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(constrainPoint.position, 0.2f);
        Gizmos.DrawSphere(otherPoint, 0.2f);
    }
}
