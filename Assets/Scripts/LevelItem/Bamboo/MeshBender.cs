using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(MeshFilter))]
public class MeshBender : MonoBehaviour
{
    public float curvatureMul = 0;
    public float targetCurvature;
    public float length = 1;
    private float bendAmount = 1; // thinking about change to bool
    [SerializeField] private bool showGizmo = true;

    public GameObject targetObject;
    private Mesh targetMesh;
    private Transform targetTransform;
    private Vector3[] targetOrigVerts;

    static int numBendSegments = 5;
    Vector3[] origPts = new Vector3[numBendSegments];
    Vector3[] pts = new Vector3[numBendSegments];
    Vector3[] normals = new Vector3[numBendSegments];
    Vector3[] binormals = new Vector3[numBendSegments];
    Vector3[] tangents = new Vector3[numBendSegments];

    private void Start()
    {
        InitiaInfo();
    }

    private void LateUpdate()
    {
        Deform();
    }

    private void InitiaInfo()
    {
        targetObject = transform.parent.gameObject;
        //length = targetObject.transform.localScale.x;
        targetMesh = targetObject.GetComponent<MeshFilter>().mesh;
        targetTransform = targetObject.transform;
        targetOrigVerts = targetMesh.vertices;
    }



    private void Deform()
    {
        //使用本地变量来避免修改原本的变量
        Mesh mesh;
        Transform trans;
        Vector3[] restPtVerts;
        Vector3[] verts;//这个目前还不知道是做什么的

        mesh = targetMesh;
        trans = targetTransform;
        restPtVerts = targetOrigVerts;

        verts = new Vector3[restPtVerts.Length];

        for (int i = 0; i < restPtVerts.Length; i++)
        {
            Vector3 wsPt = trans.TransformPoint(restPtVerts[i]); //目标mesh的每个顶点在世界坐标的位置。
            // 当component所在的local location为（0,0,0）的时候，它自身所转化的（transform)与目标trans（targetTransform）之间没有差别。
            // 考虑到原本的脚本中可能会涉及到多个物品的转化，因此对做了区别。
            float p = PtLineProjection(wsPt, transform.TransformPoint(new Vector3(0, 0, 0)), transform.TransformPoint(new Vector3(0, length, 0)));
            p = Mathf.Clamp(p, 0.0f, 1.0f);

            Vector3 proj, N, BiN, T;
            Vector3 bendEnd = trans.TransformPoint(0, length, 0);
            float tmp = Vector3.Dot(wsPt, bendEnd); //不明意义的点积。
            tmp = Mathf.Clamp(tmp, 0, bendEnd.magnitude);

            proj = transform.TransformPoint(new Vector3(0, 0, 0) + p * (new Vector3(0, length, 0) - new Vector3(0, 0, 0)));
            N = transform.TransformDirection(new Vector3(1, 0, 0));
            BiN = transform.TransformDirection(new Vector3(0, 0, -1));
            T = transform.TransformDirection(new Vector3(0, 1, 0));

            float dN, dBiN, dT;
            dN = PtLineProjection(wsPt, proj, proj + N);
            dBiN = PtLineProjection(wsPt, proj, proj + BiN);
            dT = PtLineProjection(wsPt, proj, proj + T);

            Vector3 proj2, N2, BiN2, T2;
            float x, y, z;

            if (curvatureMul != 0)
            {
                x = (Mathf.Cos(p * curvatureMul / Mathf.PI) * Mathf.PI / curvatureMul - Mathf.PI / curvatureMul) * length;
                y = (Mathf.Sin(p * curvatureMul / Mathf.PI) * Mathf.PI / curvatureMul) * length;
                z = 0;
                proj2 = transform.TransformPoint(new Vector3(x, y, z));
                N2 = transform.TransformDirection(Vector3.Normalize(new Vector3(x, y, z) - new Vector3(-Mathf.PI / curvatureMul * length, 0, 0)));
                if (curvatureMul < 0)
                {
                    N2 *= -1;
                }
                BiN2 = BiN;
                T2 = Vector3.Cross(N2, BiN2);
            }
            else
            {
                proj2 = proj;
                N2 = N;
                BiN2 = BiN;
                T2 = T;
            }

            verts[i] = Vector3.Lerp(restPtVerts[i], trans.InverseTransformPoint(proj2 + dN * N2 + dBiN *BiN2 + dT * T2), bendAmount);
        }

        mesh.vertices = verts;

        if (trans.GetComponent<MeshCollider>() != null)
        {
            trans.GetComponent<MeshCollider>().sharedMesh = mesh;
        }
    }

    private void DrawBend()
    {
        for (int i = 0; i < origPts.Length; i++)
        {
            float u = (float)i / (float)(origPts.Length - 1);
            origPts[i] = transform.TransformPoint(new Vector3(0, u * length, 0));
            normals[i] = transform.TransformDirection(new Vector3(0, 1, 0));

            float x, y, z;
            Vector3 pt, normal, binormal, tangent;

            if (curvatureMul != 0)
            {
                x = (Mathf.Cos(u * curvatureMul / Mathf.PI) * Mathf.PI / curvatureMul - Mathf.PI / curvatureMul) * length;
                y = (Mathf.Sin(u * curvatureMul / Mathf.PI) * Mathf.PI / curvatureMul) * length;
                z = 0;
                pt = transform.TransformPoint(new Vector3(x, y, z));
                normal = transform.TransformDirection(Vector3.Normalize(new Vector3(x, y, z) - new Vector3(-Mathf.PI / curvatureMul * length, 0, 0)));
                if (curvatureMul < 0)
                {
                    normal *= -1;
                }
            }
            else
            {
                pt = origPts[i];
                normal = transform.TransformDirection(new Vector3(1, 0, 0));
            }
            binormal = transform.TransformDirection(new Vector3(0, 0, -1));
            tangent = Vector3.Cross(normal, binormal);

            pts[i] = pt;
            normals[i] = normal;
            binormals[i] = binormal;
            tangents[i] = tangent;
        }

        for (int i = 0; i < pts.Length; i++)
        {

            Gizmos.color = Color.red;
            Gizmos.DrawLine(pts[i], pts[i] + .1F * normals[i] * length);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(pts[i], pts[i] + .1F * tangents[i] * length);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(pts[i], pts[i] + .1F * binormals[i] * length);

            Gizmos.color = new Color(1, 1, 1, .5F);
            if (i < pts.Length - 1)
                Gizmos.DrawLine(pts[i], pts[i + 1]);
        }
    }

    // get the precent of point projection at the PtLine
    private float PtLineProjection(Vector3 point,Vector3 start,Vector3 end)
    {
        float p = 0;
        Vector3 a = point - start;
        Vector3 b = end - start;
        //u = Vector3.Dot(a,b)/Mathf.Pow((b.magnitude),2.0f);
        p = Vector3.Dot(a, b) / Mathf.Pow(Vector3.Distance(start,end), 2.0f);
        //p = Vector3.Dot(a, b) / b.sqrMagnitude; // 使用sqrMagnitude少一步平方根计算，同时由于这个点本身就需要平方。
        
        return p;
    }

    void OnDrawGizmos()
    {
        if (showGizmo)
            DrawBend();
    }
}
