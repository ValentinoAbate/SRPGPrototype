using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats
{
    public int MaxHp { get; set; }

    public int Hp { get => hp; set => hp = Math.Min(MaxHp, value); }
    private int hp;

    public int MaxAP { get; set; }

    public int Repair { get; set; }

    public void RestoreHpToMax() => Hp = MaxHp;

    public void SetMaxValues(PlayerStats other)
    {
        MaxHp = other.MaxHp;
        MaxAP = other.MaxAP;
        Repair = other.Repair;
        if (Hp > MaxHp)
            RestoreHpToMax();
    }
}
