using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UseLimiter
{
    public enum Frequency
    {
        Battle,
        Turn,
    }

    public string DisplayName => limit == 1 ? $"once per {frequency} ({currUses}/{limit})" : $"{limit} times per {frequency} ({currUses}/{limit})";

    [SerializeField] private Frequency frequency;
    [SerializeField] private int limit;

    private int currUses;

    public bool TryUse()
    {
        if (currUses >= limit)
            return false;
        ++currUses;
        return true;
    }

    public void Attach(Shell.CompileData data)
    {
        currUses = 0;
        if(frequency == Frequency.Battle)
        {
            data.onBattleStart += ResetUses;
        }else if(frequency == Frequency.Turn)
        {
            data.onPhaseStart += ResetUses;
        }
    }

    private void ResetUses(BattleGrid grid, Unit unit)
    {
        currUses = 0;
    }
}
