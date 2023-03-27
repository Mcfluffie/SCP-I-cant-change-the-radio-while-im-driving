using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
    public GameObject window;
    public GameObject ghosthand;
    public Animator anime;


    public void Start()
    {
        anime.Play("windowidel");
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RadioTrigger"))
        {
            anime.Play("window");
            ghosthand.SetActive(false);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("RadioTrigger"))
        {
            anime.Play("windowidel");
        }
    }

}