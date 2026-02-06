using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddStatAfterActionAbilityTurnLimit : ProgramEffectAddStatAfterActionAbility
{
    [SerializeField] UseLimiter useLimiter;

    protected override void AddAbility(ref Shell.CompileData data)
    {
        base.AddAbility(ref data);
        useLimiter.Attach(data);
    }

    protected override bool AppliesToAction(Action action, int cost, Unit user, out int baseValue)
    {
        return base.AppliesToAction(action, cost, user, out baseValue) && useLimiter.TryUse();
    }

    public override bool CanSave(bool isBattle) => isBattle;

    public override string Save(bool isBattle)
    {
        return isBattle ? useLimiter.Save() : string.Empty;
    }

    public override void Load(string data, bool isBattle, Unit unit)
    {
        if (isBattle)
        {
            useLimiter.Load(data);
        }
    }
}
