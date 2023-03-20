using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
    public GameObject window;
    public GameObject ghosthand;
    public Transform pos; 
    public float targetYPosition; // the target y position for the window

    private Vector3 initialPosition; // the initial position of the window

    private void Start()
    {
        initialPosition = window.transform.position; // store the initial position of the window
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RadioTrigger"))
        {
            // Move the window down to the target position
            Vector3 newPos = window.transform.position;
            newPos.y = targetYPosition;
            window.transform.position = newPos;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("RadioTrigger"))
        {
            // Move the window back to its initial position
            window.transform.position = pos.position;
            ghosthand.SetActive(false);
        }
    }
}