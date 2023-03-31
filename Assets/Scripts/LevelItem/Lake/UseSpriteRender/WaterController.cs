using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class WaterController : MonoBehaviour
{
    private int CorsnersCount = 2;
    [SerializeField] private SpriteShapeController spriteShapeController;
    [SerializeField] private GameObject waveNodePref;
    [SerializeField] private GameObject wavePoints;
    [SerializeField] [Range(1, 100)] private int waveCount;
    public float nodeDis;

    [SerializeField] private float springStiffness = 0.1f;
    [SerializeField] private float dampening = 0.03f;
    [SerializeField] private float spread = 0.006f;
    [SerializeField] public List<WaterNode> springs = new List<WaterNode>();



    private void OnValidate()
    {
        StartCoroutine(CreateWaves());
    }

    #region Create Wave Node
    IEnumerator CreateWaves()
    {
        foreach (Transform child in wavePoints.transform)
        {
            StartCoroutine(Destory(child.gameObject));
        }
        yield return null;
        SetWaves();
        yield return null;
    }

    IEnumerator Destory(GameObject go)
    {
        yield return null;
        DestroyImmediate(go);
    }


    private void SetWaves()
    {
        Spline waterSpline = spriteShapeController.spline;
        int waterNodeCount = waterSpline.GetPointCount();

        //先删除中间的点
        for (int i = CorsnersCount; i < waterNodeCount - CorsnersCount; i++)
        {
            waterSpline.RemovePointAt(CorsnersCount);
        }

        //获得总间距
        Vector3 waterTLCorner = waterSpline.GetPosition(1);
        Vector3 waterTRCorner = waterSpline.GetPosition(2);
        float waterWidth = waterTRCorner.x - waterTLCorner.x;

        //均分建立新的点
        float spacingPerWave = waterWidth / (waveCount + 1);
        nodeDis = spacingPerWave;

        for (int i = waveCount; i > 0; i--)
        {
            int index = CorsnersCount;

            Smoothen(waterSpline, index);

            float disPos = waterTLCorner.x + spacingPerWave * i;
            Vector3 wavePoint = new Vector3(disPos, waterTLCorner.y, waterTLCorner.z);
            waterSpline.InsertPointAt(index, wavePoint);
            waterSpline.SetHeight(index, 0.1f);
            waterSpline.SetCorner(index, false);
            waterSpline.SetTangentMode(index, ShapeTangentMode.Continuous);
        }

        //把点加入list,同时完成初始化
        springs = new List<WaterNode>();
        for (int i = 0; i <= waveCount + 1; i++)
        {
            int index = i + 1;

            GameObject waveNode = Instantiate(waveNodePref, wavePoints.transform, false);
            waveNode.transform.localPosition = waterSpline.GetPosition(index);

            WaterNode waterNode = waveNode.GetComponent<WaterNode>();
            waterNode.Init(spriteShapeController,this);
            springs.Add(waterNode);
        }
    }

    private void Smoothen(Spline waterSpline, int index)
    {
        Vector3 position = waterSpline.GetPosition(index);
        Vector3 positionPrev = position;
        Vector3 positionNext = position;

        if (index > 1)
        {
            positionPrev = waterSpline.GetPosition(index - 1);
        }
        if (index - 1 <= waveCount)
        {
            positionNext = waterSpline.GetPosition(index + 1);
        }

        Vector3 forward = gameObject.transform.forward;

        float scale = Mathf.Min((positionNext - position).magnitude, (positionPrev - position).magnitude) * 0.33f;
        Vector3 leftTangent = (positionPrev - position).normalized * scale;
        Vector3 RightTangent = (positionNext - position).normalized * scale;

        SplineUtility.CalculateTangents(position, positionPrev, positionNext, forward, scale, out RightTangent, out leftTangent);
        waterSpline.SetLeftTangent(index, leftTangent);
        waterSpline.SetRightTangent(index, RightTangent);
    }

    #endregion

    #region Simulate

    private void Update()
    {
        foreach (WaterNode waterSprintComponent in springs)
        {
            waterSprintComponent.WaveSprintUpdate(springStiffness, dampening);
            waterSprintComponent.WaveNodeUpdate();
        }

        UpdateSprings();
    }

    private void UpdateSprings()
    {
        int count = springs.Count;
        float[] leftDeltas = new float[count];
        float[] rightDeltas = new float[count];

        for (int i = 0; i < count; i++)
        {
            if (i > 0)
            {
                leftDeltas[i] = spread * (springs[i].Height - springs[i - 1].Height);
                springs[i - 1].Velocity += leftDeltas[i];
            }
            if (i < springs.Count - 1)
            {
                rightDeltas[i] = spread * (springs[i].Height - springs[i + 1].Height);
                springs[i + 1].Velocity += rightDeltas[i];
            }
        }
    }

    // add force to Water Point;
    private void Splash(int index, float speed)
    {
        if (index >= 0 && index < springs.Count)
        {
            springs[index].Velocity += speed;
        }
    }
    
    #endregion
}
