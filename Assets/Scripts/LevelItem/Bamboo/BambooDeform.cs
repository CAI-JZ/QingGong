using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshBender))]
public class BambooDeform : MonoBehaviour
{
    [Header("Reference")]
    private MeshBender bender;

    [Header("Deform")]
    public float standDeform;
    public float normalDeform;
    private float currentDeform;
    [SerializeField]private float targetDefrom;
    public AnimationCurve standCurve;
    public float deformMul;
    public float deformSpeed;
    public float recoverSpeed;

    [Header("Test")]
    public Vector2 Speed;
    public float lerpTest = 0;
    public float lerpTarget;
    public float lerpSpeed;

    private void Start()
    {
        bender = GetComponent<MeshBender>();
    }

    private void LateUpdate()
    {
        bender.curvatureMul = currentDeform;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StopDeform();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            PlayerJumpTo(Speed);
            
        }
    }

    //当玩家跳跃到树上
    public void PlayerJumpTo(Vector2 Speed)
    {
        float bendDirect = Speed.x > 0 ? -1 : 1;    
        targetDefrom = (Mathf.Abs(Speed.y) * deformMul + standDeform) * bendDirect;
        StopAllCoroutines();
        StartCoroutine(SmoothlyDeformTransfer(deformSpeed));
        //StartCoroutine(DeformUseCurve(0.5f));
        
    }

    public void StopDeform()
    {
        targetDefrom = normalDeform;
        StopAllCoroutines();
        StartCoroutine(SmoothlyDeformTransfer(recoverSpeed));
    }

    private IEnumerator DeformUseCurve(float end)
    {
        float time = 0;
        
        while (time <= end)
        { 
            currentDeform =  standCurve.Evaluate(time) * standDeform;
            time += Time.deltaTime;
            yield return null;
        }
    }

    public void PlayerWalkOn()
    { 
    
    }

    private IEnumerator SmoothlyDeformTransfer(float speed)
    {
        float time = 0;
        float difference = Mathf.Abs(targetDefrom - currentDeform);
        float startValue = currentDeform;

        while(time < difference)
        {
            currentDeform = Mathf.Lerp(startValue, targetDefrom, time / difference * speed);
            time += Time.deltaTime;
            yield return null;
        }

        currentDeform = targetDefrom;
    }

    private void SmoothlyLerpCurve()
    {
        float time = 0;
        //float difference = Mathf.Abs(targetCurvature - curvatureMul);
        //float startValue = curvatureMul;
    }
}
