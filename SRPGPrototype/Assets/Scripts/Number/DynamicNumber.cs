using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DynamicNumber
{
    [SerializeField]
    private int min = int.MinValue;
    [SerializeField]
    private int max = int.MaxValue;
    [SerializeField]
    private int baseAmount = 0;
    [SerializeField]
    private int modifier = 0;
    [SerializeField]
    private int multiplier = 1;

    public int Value(int number)
    {
        int modNumber = baseAmount + ((number + modifier) * multiplier);
        return Mathf.Clamp(modNumber, min, max);
    }
}
