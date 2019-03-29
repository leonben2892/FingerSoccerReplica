using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_AudioManager : MonoBehaviour
{
    public AudioClip[] gameSounds;
    private AudioSource audioSource;

    /// <summary>
    /// Implementing SC_AudioManager as singleton design pattern.
    /// </summary>
    static SC_AudioManager instance;
    public static SC_AudioManager Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.Find("SC_AudioManager").GetComponent<SC_AudioManager>();
            return instance;
        }
    }

    void Awake(){Init();}

    /// <summary>
    /// Initialization of required variables for proper audio manager behavior.
    /// </summary>
    void Init()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Starts the game background music.
    /// </summary>
    void start()
    {
        audioSource.Play();
    }

    /// <summary>
    /// Plays a sound based on a given string.
    /// </summary>
    /// <param name="sound">Represnts the sound to be played.</param>
    public void PlaySound(string sound)
    {
        switch (sound)
        {
            case "PuckHit":
                SC_AudioManager.Instance.audioSource.PlayOneShot(gameSounds[0]);
                break;
            case "BallHit":
                SC_AudioManager.Instance.audioSource.PlayOneShot(gameSounds[1]);
                break;
            case "GoalScored":
                SC_AudioManager.Instance.audioSource.PlayOneShot(gameSounds[2]);
                break;
        }
    }
}
