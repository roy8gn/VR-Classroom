using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDistraction : Distraction
{
    AudioSource sound;

    public AudioDistraction(AudioSource asource)
    { 
        sound = asource;
    }

    public override void StartDistraction()
    {
        sound.Play();
    }


    public override void StopDistraction()
    {
        sound.Stop();
    }
}
