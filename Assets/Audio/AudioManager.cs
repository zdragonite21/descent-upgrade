using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    private void Awake()
    {
        foreach (Sound s in sounds)
        {
            AudioSource newSound = gameObject.AddComponent<AudioSource>();

            newSound.clip = s.audioClip;

            newSound.volume = s.volume;
            newSound.pitch = s.pitch;

            newSound.loop = s.isLooping;

            if (newSound.playOnAwake)
            {
                newSound.Play();
            }
        }


    }


    public void PlaySound(string name)
    {
        Sound s = Array.Find(sounds, s => s.name == name);

        if (s==null)
        {
            Debug.LogError("Sound " + name + " not found!");
            return;
        }

        s.audioSource.Play();
    }

}
