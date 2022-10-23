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


    private void Awake()
    {
        currentQi = maxQi;
        insideQi = maxQi;
    }

    private void Update()
    {
        frontBar.fillAmount = currentQi / maxQi;
        insideBar.fillAmount = insideQi / maxQi;

        if (Input.GetKeyDown(KeyCode.L))
        {
            DecreaseQi(20);
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

    private void FixedUpdate()
    {
        
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
    }
}
