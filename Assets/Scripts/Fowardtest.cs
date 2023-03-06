using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fowardtest : MonoBehaviour
{
    public float speed = 5.0f;
    

    public float forceAmount = 10f; // The amount of force to add to the object
    public ForceMode forceMode = ForceMode.Impulse; // The type of force to add
    public Transform steeringWheel; // The transform of the steering wheel object
  

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    public void OnTriggerEnter(Collider other)
    {
        // If the trigger is hit by another object
        if (other.CompareTag("RightTrigger"))
        {
            Debug.Log("I COLLIDE R");
            // Add a force to the object to make it move to the right
            rb.AddForce(Vector3.left * forceAmount, forceMode);
        }
        else if (other.CompareTag("LeftTrigger"))
        {
            Debug.Log("I COLLIDE L");
            // Add a force to the object to make it move to the right
            rb.AddForce(Vector3.right * forceAmount, forceMode);
        }
    }


    void Update()
    {
        rb.AddForce(Vector3.forward * speed * Time.deltaTime);

        // Calculate the direction of the car based on the rotation of the steering wheel
        Vector3 carDirection = Quaternion.Euler(0, steeringWheel.rotation.eulerAngles.y, 0) * Vector3.forward;

        // Apply the direction to the rigidbody of the car
        rb.velocity = carDirection * speed;
    }
} 
    

