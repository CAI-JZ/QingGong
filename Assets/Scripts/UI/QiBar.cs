using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QiBar : MonoBehaviour
{
    public Image frontBar, insideBar;
    private QiValue Qi;

    private int maxQi;
    [SerializeField] private int currentQi;
    [SerializeField] private float insideQi;

    [SerializeField] private float frontLerpSpeed;
    [SerializeField] private float insdeLerpSpeed;
    [SerializeField] private float insideWaitTime = 1f;
    private float insideLerpTimer;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        Qi = GameObject.FindGameObjectWithTag("Player").GetComponent<QiValue>();
        if (Qi != null)
        {
            InitializedData();
        }

        Qi.eventDecreaseQi += DecreaseQi;
        Qi.eventIncreaseQi += IncreaseQi;
        Qi.eventQiUpgrade += QiUpgrade;

    }

    private void InitializedData()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        maxQi = Qi.qiValue;
        currentQi = maxQi;
        insideQi = maxQi;
    }

    private void Update()
    {
        frontBar.fillAmount = (float)currentQi / maxQi;
        insideBar.fillAmount = insideQi / maxQi;
        InsideLerp();

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.E))
        {
            canvasGroup.alpha = 1;
        }
#endif
        
    }

    private void InsideLerp()
    {
        if (insideQi > currentQi)
        {
            insideLerpTimer -= Time.deltaTime;
            if (insideLerpTimer <= 0)
            {
                insideQi = Mathf.Lerp(insideQi, currentQi, insdeLerpSpeed);
            }
        }
    }

    private IEnumerator Disappear()
    {
        yield return new WaitForSeconds(2f);
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime;
            //canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, 5 * Time.deltaTime);
        }
        //canvasGroup.alpha = 0;
    }

    public void DecreaseQi(int cost)
    {
        Debug.Log("事件触发");
        int temp = currentQi - cost;
        if(temp<0)
        {
            return;   
        }
        currentQi = temp;
        canvasGroup.alpha = 1;

        insideLerpTimer = insideWaitTime;
        StopAllCoroutines();
        StartCoroutine("Disappear");
    }

    public void IncreaseQi(int value)
    {
        int temp = currentQi + value;
        if (temp > maxQi)
        {
            Debug.Log("不能再增加了");
            return;
        }
        canvasGroup.alpha = 1;
        currentQi = temp;
        insideQi = currentQi;
    }

    public void QiUpgrade(int newLevel)
    { 
    
    }
}
