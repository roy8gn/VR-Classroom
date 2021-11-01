using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoardScript : MonoBehaviour
{
    public TextMeshPro text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<TextMeshPro>();
        text.text = "Welcome to VR Classroom!";
    }

    // Update is called once per frame
    void Update()
    {
    }
}
