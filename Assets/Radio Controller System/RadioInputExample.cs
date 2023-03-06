using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is not a part of the actual system, it's just an example I have created to demonstrate
/// how you can modify the radio station using an input.
/// </summary>
public class RadioInputExample : MonoBehaviour
{
    private RadioController controller; //radio controller reference

    /// <summary>
    /// Awake is called before Start.
    /// </summary>
    private void Awake()
    {
        TryGetComponent(out controller);    //find radio controller script attached to this object
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update()
    {
        if (controller != null)
        {
            /// <summary>
            /// All you need to do is modify the CurrentStationIndex property in an instance of the radio controller.
            /// Provided that the controller is setup correctly and this script is on the same object, it should all work.
            /// In this example you change the radio station by using the mouse scroll wheel.
            /// </summary>
            #region EXAMPLE INPUTS (mouse scroll wheel)
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                controller.CurrentStationIndex += 1;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                controller.CurrentStationIndex -= 1;
            }
            #endregion
        }
    }
}
