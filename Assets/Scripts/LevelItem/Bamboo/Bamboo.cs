using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bamboo : MonoBehaviour,IBamboo,IBorrow
{
    [Header("Reference")]
    [SerializeField] private BambooCosmatic _cosmatic;
    
    [Header("Swing")]
    [SerializeField] private Transform constrainPoint;
    private float horizForcePower;
    [SerializeField] private float vertForcePower;
    [SerializeField] private float velocity;
    [SerializeField] private float forceIntert;
    [SerializeField] private float swingDampMul;
    [SerializeField] private float swingDir;
    [SerializeField] private float bambooHight;
    [SerializeField] private float forceMaxValue;
    [SerializeField] private float impluseForce;

    [Header("Test")]
    [SerializeField] private float testDir;
    


    // 末端定点永远想要去靠近的点

    [SerializeField] public List<BambooSegment> bambooSegments = new List<BambooSegment>();
    

    [Header("Bamboo Data")]
    [SerializeField] private float bambooSegLength = 1f;
    [SerializeField] public int segmentCount = 5;
    [SerializeField] public float segRadius = 1;

    private void Start()
    {
        InitBambooSege();
        if (_cosmatic != null)
        {
            _cosmatic.SetRender(bambooSegments.Count);
        }
        bambooHight = bambooSegLength * (segmentCount - 1);
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
#if UNITY_EDITOR
        GetTargetPos();
#endif
        Simulate();
        StartCoroutine(ImpluseDecrease());
    }

    public void AddImpluse(float dir)
    {
        horizForcePower = impluseForce;
        if (dir != 0){
            swingDir = dir > 0 ? 1 : -1;
        }
        else
        {
            swingDir = 0;
        }
        StartCoroutine(ImpluseDecrease());
    }

    IEnumerator ImpluseDecrease()
    {
        while (horizForcePower > 0)
        {
            horizForcePower -= Time.deltaTime * swingDampMul;
            yield return null;
        }
        horizForcePower = 0f;
        swingDir = 0;
    }

    public void AddForce(float posY, float dir)
    {
        if (dir != 0)
        {
            swingDir = dir > 0 ? 1 : -1;
        }
        else
        {
            swingDir = 0;
        }
        float force = posY / bambooHight * forceMaxValue;
        horizForcePower = force;  
    }

    float a = 0;
    private void GetTargetPos()
    {
   
        if (Input.GetMouseButtonDown(0))
        {
            AddImpluse(testDir);
        }
        if (Input.GetMouseButton(1))
        {
            a += Time.deltaTime * 5;
            
            a = Mathf.Clamp(a,0, bambooHight);
            AddForce(a, testDir);
        }
        else
        {
            StartCoroutine(ImpluseDecrease());
            a = 0;
        }

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
        for (int i = 0; i < 100; i++)
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
     
            int e = bambooSegments.Count - 1;
            BambooSegment endSeg = bambooSegments[e];
            endSeg.posNow.x = Mathf.SmoothDamp(endSeg.posNow.x, endSeg.posNow.x + swingDir * horizForcePower, ref velocity, forceIntert);
            bambooSegments[e] = endSeg;
        

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

    public void BorrowPower(float Dir)
    {
        AddImpluse(Dir);
    }

    public struct BambooSegment
    {
        public Vector2 posNow;
        public Vector2 posOld;

        public BambooSegment(Vector2 position)
        {
            posNow = position;
            posOld = position;
        }
    }
}
