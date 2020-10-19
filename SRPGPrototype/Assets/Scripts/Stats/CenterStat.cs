using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CenterStat
{
    public bool IsZero => Value == 0;
    public int Value { get => value; set => this.value = value; }
    [SerializeField] private int value;

    /// <summary>
    /// Sets the stat one closer to 0 (after being used)
    /// </summary>
    public void Use()
    {
        if (value < 0)
            ++value;
        else if (value > 0)
            --value;
    }

    public int ValueAfterXUses(int uses)
    {
        if (IsZero)
            return 0;
        if (value > 0)
            return Mathf.Max(0, value - uses);
        return Mathf.Min(0, value + uses);
    }
}
