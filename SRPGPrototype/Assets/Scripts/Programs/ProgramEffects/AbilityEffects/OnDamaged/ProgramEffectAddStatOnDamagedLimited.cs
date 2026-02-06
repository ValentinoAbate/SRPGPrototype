using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddStatOnDamagedLimited : ProgramEffectAddStatOnDamaged
{
    [SerializeField] private UseLimiter limiter;

    protected override void AddAbility(ref Shell.CompileData data)
    {
        base.AddAbility(ref data);
        limiter.Attach(data);
    }

    public override void Ability(BattleGrid grid, Unit self, Unit source, int amount)
    {
        if (!limiter.TryUse())
        {
            return;
        }
        base.Ability(grid, self, source, amount);
    }

    public override bool CanSave(bool isBattle) => isBattle;

    public override string Save(bool isBattle)
    {
        return isBattle ? limiter.Save() : string.Empty;
    }

    public override void Load(string data, bool isBattle, Unit unit)
    {
        if (isBattle)
        {
            limiter.Load(data);
        }
    }
}
