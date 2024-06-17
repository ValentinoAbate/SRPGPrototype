using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Stats
{
    [System.Flags]
    public enum RepairAbilities
    { 
        None = 0,
        BaseIsMax = 1,
        ExcessRepairDamages = 2,
    }

    public enum StatName
    { 
        None = -1,
        HP,
        MaxHP,
        AP,
        MaxAP,
        Repair,
        Power,
        Speed,
        Break,
    }

    public RepairAbilities RepairAbilityFlags { get; set; }

    public int MaxHP { get => maxHP; set { maxHP = value; HP = Math.Min(HP, value); } }
    private int maxHP = 0;

    public int HP { get => hp; set => hp = Math.Min(MaxHP, value); }
    private int hp;

    public int MaxAP { get; set; }

    public int BaseRepair { get; private set;  } = 0;

    public int Repair 
    { 
        get => repair; 
        set
        {
            if(RepairAbilityFlags.HasFlag(RepairAbilities.BaseIsMax))
            {
                repair = Mathf.Min(value, BaseRepair);
                return;
            }
            repair = value;
        }
    }

    private int repair;

    public CenterStat Power { get; set; } = new CenterStat();
    public CenterStat Speed { get; set; } = new CenterStat();
    public CenterStat Break { get; set; } = new CenterStat();

    public void RestoreHpToMax() => HP = MaxHP;

    public void DoRepair()
    {
        if(RepairAbilityFlags.HasFlag(RepairAbilities.ExcessRepairDamages) && Repair > BaseRepair)
        {
            HP = Mathf.Max(1, HP - (Repair - BaseRepair));
            Repair = BaseRepair;
            return;
        }
        HP += Repair;
        Repair = BaseRepair;
    }

    public void SetShellValues(Stats compiledStats)
    {
        MaxHP = compiledStats.MaxHP;
        MaxAP = compiledStats.MaxAP;
        BaseRepair = compiledStats.Repair;
        Repair = compiledStats.Repair;
        RepairAbilityFlags = compiledStats.RepairAbilityFlags;
    }
}
