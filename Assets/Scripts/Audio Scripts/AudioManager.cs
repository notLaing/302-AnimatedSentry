using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// ATTACHED TO: AudioManager prefab. Prefab should ONLY be placed in the start screen

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public static AudioManager instance;
    public AudioMixer mixer;

    // called right before Start() methods, so sounds can be called in Start()
    void Awake()
    {
        // don't destroy on scene change
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        // add audio source component to each sound
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.audioMixerGroup;
        }
    }

    // called first
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        Play("Crab Rave");
    }

    // play BGM
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        /*switch (SceneManager.GetActiveScene().buildIndex)//case numbers change by build
        {
            case 1:
                Stop("Theme");
                Play("Exploration1");
                break;
            case 2:
                Stop("Exploration1");
                Stop("Exploration2");
                Play("Exploration2");
                break;
        }*/
    }

    // play sound based on input name
    public void Play(string name)
    {
        // TO USE: FindObjectOfType<AudioManager>().Play(name);
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            return;
        }

        s.source.Play();
    }

    // stop playing a currently playing sound
    public void Stop(string name)
    {
        // TO USE: FindObjectOfType<AudioManager>().Stop(name);

        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            return;
        }
        s.source.Stop();
    }

    public void PlayUninterrupted(string name)
    {
        // TO USE: FindObjectOfType<AudioManager>().PlayUninterrupted(name);
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            return;
        }
        else
        {
            s.source.PlayOneShot(s.source.clip, s.source.volume);
        }
    }
}
