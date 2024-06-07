using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectModifyStat : ProgramEffect
{
    public int maxAPModifier;
    public int maxHpModifier;
    public int repairModifier;
    private Program program;

    public override void Initialize(Program program)
    {
        base.Initialize(program);
        this.program = program;
    }

    public override void ApplyEffect(ref Shell.CompileData data)
    {
        data.stats.MaxAP += maxAPModifier;
        data.stats.MaxHP += maxHpModifier;
        data.stats.Repair += repairModifier;
        if(data.modifierMap.ContainsKey(program))
        {
            foreach (var modifier in data.modifierMap[program])
            {
                var statMod = modifier as ModifierStatMod;
                if (statMod == null)
                    continue;
                statMod.Apply(this, ref data);
            }
        }
    }
}
