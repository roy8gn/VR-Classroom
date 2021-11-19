using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionBButton : VrClassButton
{
    public delegate void OptionBButtonPressedHandler();
    public event OptionBButtonPressedHandler OptionBButtonPressed;

    public override void Pressed()
    {
        base.Pressed();
        OptionBButtonPressed?.Invoke();
    }
}
