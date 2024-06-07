using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandomUtils;
using System.Linq;

public class ActionEffectGamble : ActionEffect, IDamagingActionEffect
{
    public override bool UsesPower => usedPower;
    private bool usedPower = false;
    public bool DealsDamage => successEffects.Any((e) => e.CanDealDamage) || failureEffects.Any((e) => e.CanDealDamage);
    public float SuccessChance => successChance;
    [Range(0, 1)]
    [SerializeField] private float successChance = 0.5f;
    [SerializeField] private GameObject successEffectsObj = null;
    [SerializeField] private GameObject failureEffectsObj = null;
    private ActionEffect[] successEffects;
    private ActionEffect[] failureEffects;

    private void Awake()
    {
        successEffects = successEffectsObj.GetComponents<ActionEffect>();
        if(failureEffectsObj != null)
        {
            failureEffects = failureEffectsObj.GetComponents<ActionEffect>();
        }
        else
        {
            failureEffects = new ActionEffect[0];
        }
    }

    public override void Initialize(BattleGrid grid, Action action, SubAction sub, Unit user, List<Vector2Int> targetPositions)
    {
        foreach(var effect in successEffects)
        {
            effect.Initialize(grid, action, sub, user, targetPositions);
        }
        foreach (var effect in failureEffects)
        {
            effect.Initialize(grid, action, sub, user, targetPositions);
        }
    }

    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        if((float)RandomU.instance.RandomDouble() < successChance)
        {
            foreach (var effect in successEffects)
            {
                effect.ApplyEffect(grid, action, sub, user, target, targetData);
            }
            usedPower = successEffects.Any((e) => e.UsesPower);
        }
        else
        {
            foreach (var effect in failureEffects)
            {
                effect.ApplyEffect(grid, action, sub, user, target, targetData);
            }
            usedPower = failureEffects.Length > 0 && failureEffects.Any((e) => e.UsesPower);
        }
    }

    public int BaseDamage(Action action, SubAction sub, Unit user, Queue<int> indices)
    {
        if(indices.Count <= 0)
        {
            return BaseDamage(action, sub, user, true, indices);
        }
        bool isSuccess = indices.Dequeue() == 0;
        return BaseDamage(action, sub, user, isSuccess, indices);
    }

    private int BaseDamage(Action action, SubAction sub, Unit user, bool success, Queue<int> indices)
    {
        var actionEffects = success ? successEffects : failureEffects;
        int effectIndex = indices.Count > 1 ? indices.Dequeue() : 0;
        if (effectIndex >= actionEffects.Length)
            return 0;
        if (actionEffects[effectIndex] is IDamagingActionEffect damageEffect)
            return damageEffect.BaseDamage(action, sub, user, indices);
        return 0;
    }
}
