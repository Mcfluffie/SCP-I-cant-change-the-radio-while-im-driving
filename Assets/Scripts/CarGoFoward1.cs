using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class CarGoFoward : MonoBehaviour
{
    public PathCreator pathCreator;
    public float speed = 5;
    public float offset = 0f;
    private float distanceTravelled;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftTrigger"))
        {
            offset = -10f;
        }
        else if (other.CompareTag("RightTrigger"))
        {
            offset = 10f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("LeftTrigger") || other.CompareTag("RightTrigger"))
        {
            offset = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        distanceTravelled += speed * Time.deltaTime;
        Vector3 pos = pathCreator.path.GetPointAtDistance(distanceTravelled);
        transform.position = pos + offset * transform.right;
        transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled);
    }
}
