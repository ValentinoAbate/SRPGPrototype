using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Stats
{
    public enum StatName
    { 
        HP,
        MaxHP,
        AP,
        MaxAP,
        Repair,
        Power,
        Speed,
        Defense,
    }

    public int MaxHP { get => maxHP; set { maxHP = value; HP = Math.Min(HP, value); } }
    private int maxHP = 0;

    public int HP { get => hp; set => hp = Math.Min(MaxHP, value); }
    private int hp;

    public int MaxAP { get; set; }

    private int baseRepair = 0;

    public int Repair { get; set; }

    public CenterStat Power { get; set; } = new CenterStat();
    public CenterStat Speed { get; set; } = new CenterStat();
    public CenterStat Defense { get; set; } = new CenterStat();

    public void RestoreHpToMax() => HP = MaxHP;

    public void DoRepair()
    {
        HP += Repair;
        Repair = baseRepair;
    }

    public void SetShellValues(Stats compiledStats)
    {
        MaxHP = compiledStats.MaxHP;
        MaxAP = compiledStats.MaxAP;
        baseRepair = compiledStats.Repair;
        Repair = compiledStats.Repair;
    }
}
