using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    public AudioClip[] channels;
    public AudioSource audioSource;
 

    private int currentChannel = 0;

    void Start()
    {
        audioSource.clip = channels[currentChannel];
        audioSource.Play();
        
    }

    public void NextChannel()
    {
        currentChannel = (currentChannel + 1) % channels.Length;
        audioSource.clip = channels[currentChannel];
        audioSource.Play();
       
    }

    public void PrevChannel()
    {
        currentChannel = (currentChannel - 1 + channels.Length) % channels.Length;
        audioSource.clip = channels[currentChannel];
        audioSource.Play();
       
    }

 
}
