using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionAButton : OptionButton
{
    public delegate void OptionAButtonPressedHandler();
    public event OptionAButtonPressedHandler OptionAButtonPressed;

    public override void Pressed()
    {
        base.Pressed();
        OptionAButtonPressed?.Invoke();
    }
}
