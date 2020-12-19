public class ModifierStatMod : Modifier
{
    public ProgramEffectModifyStat modifier;
    public override bool AppliesTo(Program p)
    {
        foreach(var effect in p.Effects)
        {
            var statMod = effect as ProgramEffectModifyStat;
            if (statMod == null)
                continue;
            if (AppliesTo(statMod))
                return true;
        }
        return false;
    }

    private bool AppliesTo(ProgramEffectModifyStat statMod)
    {
        if (modifier.maxAPModifier != 0 && statMod.maxAPModifier != 0)
            return true;
        if (modifier.maxHpModifier != 0 && statMod.maxHpModifier != 0)
            return true;
        if (modifier.repairModifier != 0 && statMod.repairModifier != 0)
            return true;
        return false;
    }

    public void Apply(ProgramEffectModifyStat statMod, ref Shell.CompileData data)
    {
        if (modifier.maxAPModifier != 0 && statMod.maxAPModifier != 0)
            data.stats.MaxAP += modifier.maxAPModifier; ;
        if (modifier.maxHpModifier != 0 && statMod.maxHpModifier != 0)
            data.stats.MaxHP += modifier.maxHpModifier;
        if (modifier.repairModifier != 0 && statMod.repairModifier != 0)
            data.stats.Repair += modifier.repairModifier;
    }
}
