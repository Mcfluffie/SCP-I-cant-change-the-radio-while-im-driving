using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class CarGoFoward : MonoBehaviour
{
    public PathCreator pathCreator;
    public float speed = 5;
    float distancetravelled;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        distancetravelled += speed * Time.deltaTime;
        transform.position = pathCreator.path.GetPointAtDistance(distancetravelled);
        transform.rotation = pathCreator.path.GetRotationAtDistance(distancetravelled);
    }

}
