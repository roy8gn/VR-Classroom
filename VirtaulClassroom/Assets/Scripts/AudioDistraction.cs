using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDistraction : Distraction
{
    AudioSource sound;

    public AudioDistraction(AudioSource asource)
    {
        //this.type = 'Auditory'; 
        sound = asource;
    }

    public override void InitDistraction()
    {
        sound.Play();
    }
}
