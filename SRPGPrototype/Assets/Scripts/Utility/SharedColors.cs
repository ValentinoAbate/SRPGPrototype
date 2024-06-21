using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SharedColors
{
    private const float disabledColorVal = 0.7843137f;
    public static Color DisabledColor { get; } = new Color(disabledColorVal, disabledColorVal, disabledColorVal, 0.5019608f);
}
