using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionButton : VrClassButton
{
    public delegate void OptionAButtonPressedHandler();
    public event OptionAButtonPressedHandler OptionAButtonPressed;
    [SerializeField] public string ButtonName;

    public override void Pressed()
    {
        base.Pressed();
        OptionAButtonPressed?.Invoke();
    }
}

