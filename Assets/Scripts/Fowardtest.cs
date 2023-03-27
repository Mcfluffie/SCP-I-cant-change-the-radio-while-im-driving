using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fowardtest : MonoBehaviour
{
    public float speed = 5.0f;
    public float maxSteeringAngle = 45f; // The maximum angle the wheels can turn
    public float rotationSpeed = 10f; // The speed at which the car rotates

    public float forceAmount = 10f; // The amount of force to add to the object
    public ForceMode forceMode = ForceMode.Impulse; // The type of force to add
    public Transform steeringWheel; // The transform of the steering wheel object

    private Rigidbody rb;
    private Quaternion targetRotation; // The target rotation of the car
    public float maxMagnitude = 2.0f; // maximum magnitude for carDirection

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        targetRotation = transform.rotation; // Initialize the target rotation to the current rotation of the car
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
        carDirection = Vector3.ClampMagnitude(carDirection, maxMagnitude);

        // Apply the direction to the rigidbody of the car
        rb.velocity = carDirection * speed;

        // Calculate the target rotation based on the steering angle
        float steeringAngle = Mathf.Clamp(steeringWheel.localRotation.eulerAngles.z, -maxSteeringAngle, maxSteeringAngle);
        targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, steeringWheel.rotation.eulerAngles.y + steeringAngle, transform.rotation.eulerAngles.z);

        // Rotate the car towards the target rotation using lerp
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed + Time.deltaTime);
    }
}


