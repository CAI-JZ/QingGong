using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bamboo : MonoBehaviour
{
    [Header("Swing")]
    [SerializeField] private Transform constrainPoint;
    [SerializeField] private float horizForcePower;
    [SerializeField] private float vertForcePower;
    [SerializeField] private float velocity;
    [SerializeField] private float forceIntert;

    [SerializeField] private BambooCosmatic _cosmatic;

    bool isAddForce;

    // 末端定点永远想要去靠近的点

    [SerializeField] public List<BambooSegment> bambooSegments = new List<BambooSegment>();
    

    [Header("Bamboo Data")]
    [SerializeField] private float bambooSegLength = 1f;
    [SerializeField] public int segmentCount = 5;
    [SerializeField] public GameObject bambooNodePrefab;

    [Header("Test")]
    [SerializeField] public float segRadius = 1;

    private void Start()
    {
        InitBambooSege();
        if (_cosmatic != null)
        {
            _cosmatic.SetRender(bambooSegments.Count);
        }
    }

    private void InitBambooSege()
    {
        Vector3 firstPointPos = constrainPoint.position;

        // 初始化所有竹节的节点
        for (int i = 0; i < segmentCount; i++)
        {
            bambooSegments.Add(new BambooSegment(firstPointPos));
            firstPointPos.y += bambooSegLength;
            //GameObject node = Instantiate(bambooNodePrefab,)
            
        }
    }

    private void Update()
    {
        GetTargetPos();
        Simulate();
    }

    private void GetTargetPos()
    {
        if (Input.GetMouseButton(0))
        {
            //force = Mathf.Lerp(force, horizForcePower, Time.time * forceIntert);
            isAddForce = true;
        }
        else
        {
            
            isAddForce = false;
        }
        //mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    // Debug展示使用
    private void OnDrawGizmos()
    {
        if(bambooSegments.Count <= 0)
        {
            InitBambooSege();
            //return;
        }

        for (int i = 0; i < bambooSegments.Count; i++)
        {
            float t = 0;
            t = (float)i/bambooSegments.Count;
            Gizmos.color = Color.LerpUnclamped(Color.red, Color.yellow, t);
            Gizmos.DrawSphere(bambooSegments[i].posNow, segRadius);
            if (i > 0)
            {
                //Gizmos.color = new Color(i * 100, 1, 1, 1);
                Gizmos.DrawLine(bambooSegments[i-1].posNow, bambooSegments[i].posNow);
            } 
        }
    }

    private void Simulate()
    {
        
        Vector2 upForce = Vector2.up * vertForcePower;

        for (int i = 0; i < bambooSegments.Count; i++)
        {
            BambooSegment segment = bambooSegments[i];
            Vector2 velocity = segment.posNow - segment.posOld;
            segment.posOld = segment.posNow;
            segment.posNow += velocity;
            segment.posNow += upForce * Time.deltaTime;
            bambooSegments[i] = segment;
        }

        //Constraint
        for (int i = 0; i < 50; i++)
        {
            ApplyConstraint();
        }

    }

    private void ApplyConstraint()
    {
        //约束起始点
        BambooSegment rootSeg = bambooSegments[0];
        rootSeg.posNow = constrainPoint.position;
        bambooSegments[0] = rootSeg;

        // 在鼠标点击的时候，设置最后点的坐标
        if (isAddForce)
        {
            int e = bambooSegments.Count - 1;
            BambooSegment endSeg = bambooSegments[e];
            endSeg.posNow.x = Mathf.SmoothDamp(endSeg.posNow.x, endSeg.posNow.x + horizForcePower, ref velocity, forceIntert);
            bambooSegments[e] = endSeg;
        }

        // 点之间的约束
        for (int i = 0; i < bambooSegments.Count - 1; i++)
        { 
            BambooSegment firstSeg = bambooSegments[i];
            BambooSegment secondSeg = bambooSegments[i + 1];

            float dis = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float error = Mathf.Abs(dis - bambooSegLength);
            Vector2 changeDir = Vector2.zero;

            if (dis > bambooSegLength)
            {
                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            }
            else if (dis < bambooSegLength)
            {
                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
            }

            Vector2 changeAmount = changeDir * error;
            if (i != 0)
            {
                firstSeg.posNow -= changeAmount * 0.5f;
                bambooSegments[i] = firstSeg;
                secondSeg.posNow += changeAmount * 0.5f;
                bambooSegments[i + 1] = secondSeg;
            }
            else
            {
                secondSeg.posNow += changeAmount;
                bambooSegments[i + 1] = secondSeg;
            }

        }
    }

    public struct BambooSegment
    {
        public Vector2 posNow;
        public Vector2 posOld;
        //public GameObject bambooNode;

        public BambooSegment(Vector2 position)
        {
            posNow = position;
            posOld = position;
            //bambooNode = Instantiate(prefab, position,Quaternion.identity, parent);
        }
    }
}
