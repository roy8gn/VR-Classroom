using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardReactToButton : Board
{
    [SerializeField] private StartButton startButton;
    [SerializeField] private OptionAButton optionAButton;

    void Start()
    {
        startButton.StartButtonPressed += OnStartButtonPressed;
        optionAButton.OptionAButtonPressed += OnOptionAButtonPressed;
    }

    public void OnDestroy()
    {
        startButton.StartButtonPressed -= OnStartButtonPressed;
        //optionAButton.OptionAButtonPressed -= OnOptionAButtonPressed;
    }
}
