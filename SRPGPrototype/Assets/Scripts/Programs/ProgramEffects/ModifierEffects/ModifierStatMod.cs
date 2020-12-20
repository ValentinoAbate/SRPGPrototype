public class ModifierStatMod : Modifier
{
    public int maxAPModifier;
    public int maxHpModifier;
    public int repairModifier;
    protected override bool ModFilter(Program p)
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
        if (maxAPModifier != 0 && statMod.maxAPModifier != 0)
            return true;
        if (maxHpModifier != 0 && statMod.maxHpModifier != 0)
            return true;
        if (repairModifier != 0 && statMod.repairModifier != 0)
            return true;
        return false;
    }

    public void Apply(ProgramEffectModifyStat statMod, ref Shell.CompileData data)
    {
        if (maxAPModifier != 0 && maxAPModifier != 0)
            data.stats.MaxAP += maxAPModifier; ;
        if (maxHpModifier != 0 && statMod.maxHpModifier != 0)
            data.stats.MaxHP += maxHpModifier;
        if (repairModifier != 0 && statMod.repairModifier != 0)
            data.stats.Repair += repairModifier;
    }
}
