using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BambooCosmatic : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Bamboo _bamboo;

    [Header("Collider")]
    EdgeCollider2D _edgeCollider;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _bamboo = gameObject.transform.parent.GetComponentInChildren<Bamboo>();
    }

    private void Start()
    {
        _edgeCollider = GetComponent<EdgeCollider2D>();
    }

    private void Update()
    {
        UpdateNodePos();
        SetEdgeCollider();
    }

    public void SetRender(int count)
    {
        _lineRenderer.positionCount = count;
        UpdateNodePos();
    }

    private void UpdateNodePos()
    {
        int count = _lineRenderer.positionCount;
        for (int i = 0; i < count; i++)
        {
            _lineRenderer.SetPosition(i, _bamboo.bambooSegments[i].posNow);
        }
    }

    private void SetEdgeCollider()
    {
        if (_edgeCollider == null)
        {
            return;
        }

        List<Vector2> edges = new List<Vector2>();

        for (int i = 0; i < _lineRenderer.positionCount; i++)
        {
            Vector3 lineRenderPoint = _lineRenderer.GetPosition(i) - _lineRenderer.transform.position;
            edges.Add(lineRenderPoint);
        }

        _edgeCollider.SetPoints(edges);
    }

}
