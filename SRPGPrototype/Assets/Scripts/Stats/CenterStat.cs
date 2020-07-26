using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CenterStat
{
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
}
