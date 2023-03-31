using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BambooCosmatic : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Bamboo _bamboo;


    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _bamboo = gameObject.transform.parent.GetComponentInChildren<Bamboo>();

    }

    private void Update()
    {
        SetNodePos();
    }

    public void SetRender(int count)
    {
        _lineRenderer.positionCount = count;
        SetNodePos();
    }

    private void SetNodePos()
    {
        int count = _lineRenderer.positionCount;
        for (int i = 0; i < count; i++)
        {
            _lineRenderer.SetPosition(i, _bamboo.bambooSegments[i].posNow);
        }
    }

}
