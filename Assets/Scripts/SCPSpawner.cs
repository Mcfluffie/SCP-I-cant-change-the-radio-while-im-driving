using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCPSpawner : MonoBehaviour
{

    public GameObject[] objectsToActivate;
    public Collider triggerZone;

    public void OnTriggerEnter(Collider other)
    {
        // If the trigger is hit by the player
        if (other.CompareTag("Player"))
        {
            // Select a random GameObject from the array
            int index = Random.Range(0, objectsToActivate.Length);
            GameObject objToActivate = objectsToActivate[index];

            // Activate the selected GameObject
            objToActivate.SetActive(true);

            // Disable the trigger so it can't be activated again
            triggerZone.enabled = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject obj in objectsToActivate)
        {
            obj.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
