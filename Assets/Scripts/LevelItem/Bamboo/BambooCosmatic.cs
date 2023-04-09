using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BambooCosmatic : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Bamboo _bamboo;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        if (_lineRenderer != null)
        {
            Debug.Log("get renderer");
        }
        else
        {
            Debug.Log("do not get renderer");
        }
        _bamboo = gameObject.transform.parent.GetComponentInChildren<Bamboo>();
    }

    private void Start()
    {
        
       

    }

    private void Update()
    {
        SetNodePos();
    }

    public void SetRender(int count)
    {
        _lineRenderer.positionCount = count;
        Debug.Log("point Count£º " + _lineRenderer.positionCount);
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
