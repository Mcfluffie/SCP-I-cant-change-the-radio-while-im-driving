using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarGoFoward : MonoBehaviour
{

    public GameObject car;
    public Rigidbody rb;
    public float Speed;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //rb.velocity = transform.forward * Speed * Time.deltaTime;
        rb.AddForce(transform.forward * Speed);
    }
}
