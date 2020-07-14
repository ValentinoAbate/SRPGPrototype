using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectModifyStat : ProgramEffect
{
    public int maxAPModifier;
    public int maxHpModifier;
    public int repairModifier;

    public override void ApplyEffect(Program program, ref PlayerStats stats, ref List<Player.ProgramAction> actions)
    {
        stats.MaxAP += maxAPModifier;
        stats.MaxHp += maxHpModifier;
        stats.Repair += repairModifier;
    }
}
