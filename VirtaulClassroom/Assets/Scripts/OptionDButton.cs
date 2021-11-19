using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionDButton : VrClassButton
{
    public delegate void OptionDButtonPressedHandler();
    public event OptionDButtonPressedHandler OptionDButtonPressed;

    public override void Pressed()
    {
        base.Pressed();
        OptionDButtonPressed?.Invoke();
    }
}
