using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanshiKill : MonoBehaviour
{
    public Camera mainCamera;
    public float maxDistance = 10f;
    public GameObject objectToDestroy;

    void Update()
    {
        // Cast a ray from the camera forward
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, maxDistance))
        {
            // If the ray hits the object to be destroyed, destroy it
            if (hit.collider.gameObject == objectToDestroy)
            {
                Destroy(objectToDestroy);
            }
        }
    }
}
