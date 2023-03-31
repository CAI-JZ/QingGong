using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField] private Transform constrainPoint;
    private LineRenderer lineRender;
    private List<RopeSegment> ropeSegments = new List<RopeSegment>();
    [SerializeField] private float ropeSegLen = 0.25f;
    [SerializeField] private int segmentLength = 35;
    private float lineWidth = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        lineRender = GetComponent<LineRenderer>();
        Vector3 ropeStartPoint = constrainPoint.position;

        for (int i = 0; i < segmentLength; i++)
        {
            ropeSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y -= ropeSegLen;
        }
    }

    // Update is called once per frame
    void Update()
    {

        DrawRope();
        Simulate();
        
        
    }

    private void FixedUpdate()
    {
        //Simulate();
    }

    private void DrawRope()
    {
        // 设置头尾宽度
        float width = lineWidth;
        lineRender.startWidth = width;
        lineRender.endWidth = width;

        // 获取每个片段点的位置 
        Vector3[] ropePositions = new Vector3[segmentLength];
        for (int i = 0; i < segmentLength; i++)
        {
            ropePositions[i] = ropeSegments[i].posNow;
        }

        // 设置位置到lineRender；
        lineRender.positionCount = ropePositions.Length;
        lineRender.SetPositions(ropePositions);
    }

    private void Simulate()
    {
        Vector2 forceGravity = new Vector2(0f, -1f);

        for (int i = 0; i < segmentLength; i++)
        {
                // 更新新的 posOld 和 posNow；
                RopeSegment segment = ropeSegments[i];
                Vector2 velocity = segment.posNow - segment.posOld;
                segment.posOld = segment.posNow;
                segment.posNow += velocity;
                segment.posNow += forceGravity * Time.deltaTime;
                ropeSegments[i] = segment;
        }

        // constriant
        for (int i = 0; i < 50; i++)
        {
            ApplyConstraint();
        }

    }

    private void ApplyConstraint()
    {
        //RopeSegment firstSegment = ropeSegments[0];
        //firstSegment.posNow = constrainPoint.position;
        //ropeSegments[0] = firstSegment;

        RopeSegment firstSegment = ropeSegments[0];
        firstSegment.posNow = constrainPoint.position;
        ropeSegments[0] = firstSegment;

        for (int i = 0; i < segmentLength -1; i++)
        {
            RopeSegment firstSeg = ropeSegments[i];
            RopeSegment secondSeg = ropeSegments[i + 1];

            //获得当前两点的距离以及误差
            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float error = Mathf.Abs(dist - ropeSegLen);
            Vector2 changeDir = Vector2.zero;

            if (dist > ropeSegLen)
            {
                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            }
            else if (dist < ropeSegLen)
            {
                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
            }

            Vector2 changeAmount = changeDir * error;
            if (i != 0)
            {
                firstSeg.posNow -= changeAmount * 0.5f;
                ropeSegments[i] = firstSeg;
                secondSeg.posNow += changeAmount * 0.5f;
                ropeSegments[i + 1] = secondSeg;
            }
            else
            {
                secondSeg.posNow += changeAmount;
                ropeSegments[i + 1] = secondSeg;
            }
        }
    }

    public struct RopeSegment
    {
        public Vector2 posNow;
        public Vector2 posOld;

        public RopeSegment(Vector2 pos)
        {
            posNow = pos;
            posOld = pos;
        }
    }
}
