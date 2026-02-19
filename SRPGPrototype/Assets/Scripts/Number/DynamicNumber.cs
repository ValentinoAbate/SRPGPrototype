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
    [SerializeField]
    private bool negateNumber;

    public int Value(int number)
    {
        int modNumber = number;
        if (negateNumber)
        {
            modNumber *= -1;
        }
        modNumber = baseAmount + ((modNumber + modifier) * multiplier);
        return Mathf.Clamp(modNumber, min, max);
    }
}
