using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats
{
    public int MaxHp { get; set; }

    public int Hp { get => hp; set => hp = Math.Min(MaxHp, value); }
    private int hp;

    public int MaxAP { get; set; }

    private int baseRepair = 0;

    public int Repair { get; set; }

    public void RestoreHpToMax() => Hp = MaxHp;

    public void DoRepair()
    {
        MaxHp += Repair;
        Repair = baseRepair;
    }

    public void SetShellValues(Stats compiledStats)
    {
        MaxHp = compiledStats.MaxHp;
        MaxAP = compiledStats.MaxAP;
        baseRepair = compiledStats.Repair;
        Repair = compiledStats.Repair;
        if (Hp > MaxHp)
            RestoreHpToMax();
    }
}
