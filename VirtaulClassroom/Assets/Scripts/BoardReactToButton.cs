using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardReactToButton : Board
{
    [SerializeField] private StartButton startButton;

    void Start()
    {
        startButton.StartButtonPressed += OnStartButtonPressed;
    }

    public void OnDestroy()
    {
        startButton.StartButtonPressed -= OnStartButtonPressed;
    }
}
