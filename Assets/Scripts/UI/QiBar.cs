using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QiBar : MonoBehaviour
{
    public Image frontBar, insideBar;

    private float maxQi = 100;
    [SerializeField]private float currentQi;
    [SerializeField]private float insideQi;

    [SerializeField] private float frontLerpSpeed;
    [SerializeField] private float insdeLerpSpeed;
    [SerializeField] private float insideWaitTime = 1f;
    private float insideLerpTimer;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        currentQi = maxQi;
        insideQi = maxQi;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        frontBar.fillAmount = currentQi / maxQi;
        insideBar.fillAmount = insideQi / maxQi;

        if (Input.GetKeyDown(KeyCode.E))
        {
            canvasGroup.alpha = 1;
            DecreaseQi(20);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            canvasGroup.alpha = 1;
            IncreaseQi();
        }

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

    public void DecreaseQi(float cost)
    {
        float tempQi = currentQi - cost;
        if (tempQi > 0)
        {
            currentQi = tempQi;
        }
        else
        {
            currentQi = 0;
        }
        insideLerpTimer = insideWaitTime;
        StopAllCoroutines();
        StartCoroutine("Disappear");
    }

    public void IncreaseQi()
    {
        currentQi += 20f;
        insideQi = currentQi;
    }
}
