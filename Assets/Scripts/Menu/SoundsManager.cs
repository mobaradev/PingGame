using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    public AudioSource audioData;

    void Start()
    {
        audioData = GetComponent<AudioSource>();
        
        Debug.Log("started");
    }

    public void PlaySound()
    {
        string audioMode = PlayerPrefs.GetString("audio_music");
        if (audioMode == "full")
        {
            audioData.volume = 0.6f;
            audioData.Play(0);
        } else if (audioMode == "quiet")
        {
            audioData.volume = 0.2f;
            audioData.Play(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
