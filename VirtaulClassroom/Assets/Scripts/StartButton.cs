using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : VrClassButton
{
    public delegate void StartButtonPressedHandler();
    public event StartButtonPressedHandler StartButtonPressed;

    public override void Pressed()
    {
        base.Pressed();
        StartButtonPressed?.Invoke();
    }
}
