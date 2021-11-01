using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class ClockScript : MonoBehaviour
{
    public TextMeshPro text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DateTime now = DateTime.Now;
        string hours = "" + now.Hour;
        string mintues;
        if (now.Minute < 10)
            mintues = "0" + now.Minute;
        else
            mintues = "" + now.Minute;

        string time = hours + ":" + mintues;
        text.text = time;
    }
}
