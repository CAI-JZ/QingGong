using System;
using System.Collections;
using UnityEngine;

public class QiValue : MonoBehaviour
{
    [SerializeField]
    private int qiLevel;
    public float qiValue => qiLevel;
    [SerializeField] private float currentQiValue;
    [SerializeField] private float continDeMul;

    public event Action<float> eventDecreaseQi;
    public event Action<float> eventIncreaseQi;
    public event Action<int> eventQiUpgrade;

    private void Awake()
    {
        currentQiValue = qiLevel;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            ContinDecreaseQi();
        }
        else if(Input.GetKey(KeyCode.E))
        {
            AutoRechargeQi();
        }
    }

    public bool DecreaseQi(float cost)
    {
        float targetValue = currentQiValue - cost;
        if (targetValue < 0)
        {
            Debug.Log("气力值已耗尽，无法使用");
            return false;
        }
        currentQiValue = targetValue;
        eventDecreaseQi?.Invoke(cost);
        return true;
    }

    //待修改
    public bool ContinDecreaseQi()
    {
        float targetValue = currentQiValue - Time.deltaTime * continDeMul;
        if (targetValue < 0)
        {
            return false;
        }
        currentQiValue = targetValue;
        return true;
    }

    public void IncreaseQi(float increase)
    {
        float targetValue = increase + currentQiValue;
        if (targetValue > qiLevel)
        {
            Debug.Log("气力值已满，无需增加");
            return;
        }
        currentQiValue = targetValue;
        eventIncreaseQi?.Invoke(increase);
    }

    public void AutoRechargeQi()
    {
        if (currentQiValue > qiLevel)
        {
            currentQiValue = qiLevel;
            return;
        }
        StopCoroutine("AutoRecharge");
        StartCoroutine("AutoRecharge");
    }

    private IEnumerator AutoRecharge()
    {
        yield return new WaitForSeconds(1);
        int qiGrade = (int)currentQiValue + 1;
        while (currentQiValue < qiGrade)
        {
            currentQiValue += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        currentQiValue = qiGrade;
    }


    public void QiUpgrade(int value)
    {
        qiLevel += value;
        currentQiValue = qiLevel;
        eventQiUpgrade?.Invoke(qiLevel);
    }
}
