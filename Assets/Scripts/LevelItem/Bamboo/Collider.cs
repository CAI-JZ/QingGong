using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class Collider : MonoBehaviour
{
    [Header("Referenece")]
    public LineRenderer _lineRenderer;

    private Vector3 headPosition;
    private Vector3 lastPosition;
    private int positionCont = 0;

    PolygonCollider2D _polygonCollider;

    private void Awake()
    {
        _polygonCollider = GetComponent<PolygonCollider2D>();
    }

}
