using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This script is not a part of the actual system, it's just an example I have created to demonstrate
/// how you can modify the radio station using an input.
/// </summary>
public class RadioInputExample : MonoBehaviour
{
    private RadioController controller; //radio controller reference
    private bool triggerEntered = false;

    /// <summary>
    /// Awake is called before Start.
    /// </summary>
    private void Awake()
    {
        TryGetComponent(out controller);    //find radio controller script attached to this object

        if (TryGetComponent(out controller))
        {
            Debug.Log("RadioController found");
        }
        else
        {
            Debug.Log("RadioController not found");
        }
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update()
    {

        if (controller != null && triggerEntered)
        {
            Debug.Log("NEXT");
            controller.CurrentStationIndex += 1;
            triggerEntered = false;
        }
    }
    public void OnTriggerEnter(Collider other)
    {

        Debug.Log("OnTriggerEnter");

        if (controller != null && other.CompareTag("RadioTrigger"))
        {
            Debug.Log("NEXT");
            controller.CurrentStationIndex += 1;
        }


       
    }
}
