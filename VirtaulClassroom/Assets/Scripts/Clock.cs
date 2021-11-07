using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class Clock : MonoBehaviour
{
    public TextMeshPro text;
    public string hours;
    public string mintues;
    public DateTime now;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        now = DateTime.Now;
        text.text = now.Minute < 10 ? String.Format($"{now.Hour}:0{now.Minute}") : String.Format($"{now.Hour}:{now.Minute}");
    }
}
