using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardReactToButton : Board
{
    [SerializeField] private StartButton startButton;
    [SerializeField] private OptionAButton OptionAButton;

    void Start()
    {
        startButton.StartButtonPressed += OnStartButtonPressed;
    }

    public void OnDestroy()
    {
        startButton.StartButtonPressed -= OnStartButtonPressed;
    }
}
