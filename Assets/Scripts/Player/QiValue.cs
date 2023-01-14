using System;
using UnityEngine;

public class QiValue : MonoBehaviour
{
    [SerializeField]
    private int maxQiValue;
    public int qiValue => maxQiValue;
    [SerializeField] private int currentQiValue;
    [SerializeField] private float continDeMul;

    public event Action<int> eventDecreaseQi;
    public event Action<int> eventIncreaseQi;
    public event Action<int> eventQiUpgrade;

    private void Awake()
    {
        currentQiValue = maxQiValue;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            ContinDecreaseQi();
        }
    }

    public bool DecreaseQi(int cost)
    {
        int targetValue = currentQiValue - cost;
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

        return true;
    }

    public void IncreaseQi(int increase)
    {
        int targetValue = increase + currentQiValue;
        if (targetValue > maxQiValue)
        {
            Debug.Log("气力值已满，无需增加");
            return;
        }
        currentQiValue = targetValue;
        eventIncreaseQi?.Invoke(increase);
    }

    public void QiUpgrade(int value)
    {
        maxQiValue += value;
        currentQiValue = maxQiValue;
        eventQiUpgrade?.Invoke(maxQiValue);
    }
}
