using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;


public class Recommendation
{
    public DateTime CurrentDate;
    public Dictionary<DistractionTypeForLesson, string> RecommendationDictionary { get; set; }


    public Recommendation()
    {
        CurrentDate = DateTime.Today;
        RecommendationDictionary = new Dictionary<DistractionTypeForLesson, string>();
    }
}
