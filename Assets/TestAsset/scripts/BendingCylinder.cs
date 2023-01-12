using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BendingCylinder : MonoBehaviour
{
    Mesh _mesh;
    Vector3[] vertices;

    // Start is called before the first frame update
    void Start()
    {
        _mesh = GetComponent<MeshFilter>().mesh;
    }

    // Update is called once per frame
    void Update()
    {
        Bending();
    }

    //不太行，但是可以考虑优化
    private void Bending()
    {
        vertices = _mesh.vertices;
        Debug.Log(vertices.Length);
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] += Vector3.right * Time.deltaTime * i;
        }
        _mesh.vertices = vertices;
        _mesh.RecalculateBounds();
    }
}
