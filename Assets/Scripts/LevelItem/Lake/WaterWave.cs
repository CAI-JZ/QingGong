using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterWave : MonoBehaviour
{
    [Range(0, 1)]
    [SerializeField] private float alpha;
    [SerializeField] private Color color;
    [SerializeField] private int halfCount = 1;

    private int centerIndex;
    private LineRenderer _lineRenderer;
    private WaterController _waterController;


    public void Initialization(WaterController wc, int index)
    {
        _lineRenderer = GetComponent<LineRenderer>();
        int count = halfCount * 2 + 1;
        _lineRenderer.positionCount = count;
        _waterController = wc;
        centerIndex = index;

        Debug.Log(centerIndex);
        WavePosUpdate();

        //Invoke("AlphaUpdate", 5f);

    }

    private void Update()
    {
        if (alpha < 0)
        {
            DestroyImmediate(transform.parent.gameObject);
        }
        if (_lineRenderer != null)
        {
            //WavePosUpdate();
        }
        
    }

    private void WavePosUpdate()
    {
        int count = _lineRenderer.positionCount;
        int firstIndex = centerIndex - halfCount;
        Debug.Log(firstIndex);


        for (int i = 0; i < count; i++)
        {

            Vector3 pos = _waterController.springs[firstIndex + i].transform.position;
            _lineRenderer.SetPosition(i, pos);
        }


    }

    private void AlphaUpdate()
    {
        StartCoroutine(FadeOff());
    }

    IEnumerator FadeOff()
    {

        while (alpha > 0)
        {
            color.a = alpha;
            _lineRenderer.endColor = color;
            _lineRenderer.startColor = color;

            alpha -= Time.deltaTime;
            yield return null;
        }

    }
}
