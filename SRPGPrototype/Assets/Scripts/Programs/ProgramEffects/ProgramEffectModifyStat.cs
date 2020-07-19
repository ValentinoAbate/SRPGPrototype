using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectModifyStat : ProgramEffect
{
    public int maxAPModifier;
    public int maxHpModifier;
    public int repairModifier;

    public override void ApplyEffect(Program program, ref Shell.CompileData data)
    {
        data.stats.MaxAP += maxAPModifier;
        data.stats.MaxHp += maxHpModifier;
        data.stats.Repair += repairModifier;
    }
}
