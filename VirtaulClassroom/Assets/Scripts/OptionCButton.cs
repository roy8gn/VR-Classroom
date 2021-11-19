using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionCButton : VrClassButton
{
    public delegate void OptionCButtonPressedHandler();
    public event OptionCButtonPressedHandler OptionCButtonPressed;

    public override void Pressed()
    {
        base.Pressed();
        OptionCButtonPressed?.Invoke();
    }
}
