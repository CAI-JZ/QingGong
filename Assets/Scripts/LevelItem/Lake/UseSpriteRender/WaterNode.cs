using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class WaterNode : MonoBehaviour
{
    private float velocity = 0;
    private float force = 0;
    private float currentHeight = 0;
    private float targetHeight = 0;

    private static SpriteShapeController _shapeController = null;
    private WaterController _waterController = null;

    [SerializeField] private float resistance = 1;
    [SerializeField] private int waveIndex = 0;
    //[SerializeField] private GameObject wavePerfab;

    public float Height => currentHeight;
    public float Velocity
    {
        get { return velocity; }
        set { velocity = value; }
    }

    public void Init(SpriteShapeController ssc, WaterController wc)
    {
        var index = transform.GetSiblingIndex();
        waveIndex = index + 1;

        _shapeController = ssc;
        _waterController = wc;
        velocity = 0;
        currentHeight = transform.localPosition.y;
        targetHeight = transform.localPosition.y;
    }

    public void WaveSprintUpdate(float sprintStiffness,float dampening)
    {
        currentHeight = transform.localPosition.y;

        float dis = currentHeight - targetHeight;
        float loss = -dampening * velocity;

        force = - sprintStiffness * dis + loss;
        velocity += force;

        transform.localPosition = new Vector3(transform.localPosition.x, currentHeight + velocity, transform.localPosition.z);
    }

    public void WaveNodeUpdate()
    {
        if (_shapeController != null)
        {
            Spline waveSpline = _shapeController.spline;
            Vector3 wavePos = waveSpline.GetPosition(waveIndex);
            waveSpline.SetPosition(waveIndex, new Vector3(wavePos.x, transform.localPosition.y, wavePos.z));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            var controller = collision.GetComponent<MovementController>();
            float speed = controller.Velocity.y;

            velocity += speed / resistance;

            //Instantiate(wavePerfab, transform.position, new Quaternion(0, 0, 0, 0), transform);
            //wavePerfab.GetComponentInChildren<WaterWave>().Initialization(_waterController, waveIndex);
        }
    }
}
