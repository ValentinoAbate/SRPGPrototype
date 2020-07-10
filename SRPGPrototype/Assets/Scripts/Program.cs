using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Program : GridObject
{
    [System.Flags]
    public enum ProgColors
    { 
        White,
        Pink,
        Blue,
        Green,
    }

    public ProgColors colors;
    public Pattern shape;
}
