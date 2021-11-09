using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionBButton : OptionButton
{
    public delegate void OptionBButtonPressedHandler();
    public event OptionBButtonPressedHandler OptionBButtonPressed;

    public override void Pressed()
    {
        base.Pressed();
        OptionBButtonPressed?.Invoke();
    }
}
