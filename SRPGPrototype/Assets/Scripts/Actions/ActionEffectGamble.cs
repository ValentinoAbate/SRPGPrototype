using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandomUtils;
using System.Linq;

public class ActionEffectGamble : ActionEffect, IDamagingActionEffect, IGambleActionEffect
{
    public bool? GambleSuccess { get; private set; } = null;
    public override bool UsesPower => usedPower;
    private bool usedPower = false;
    public bool DealsDamage => successEffects.Any((e) => e.CanDealDamage) || failureEffects.Any((e) => e.CanDealDamage);
    public float SuccessChance => successChance;
    [Range(0, 1)]
    [SerializeField] private float successChance = 0.5f;
    [SerializeField] private GameObject successEffectsObj = null;
    [SerializeField] private GameObject failureEffectsObj = null;
    [SerializeField] private bool allowFailureEffectsForValidTargetPosChecks = false;
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
            failureEffects = System.Array.Empty<ActionEffect>();;
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
            usedPower = successEffects.Any(ActionEffectUsesPower);
            GambleSuccess = true;
        }
        else
        {
            foreach (var effect in failureEffects)
            {
                effect.ApplyEffect(grid, action, sub, user, target, targetData);
            }
            usedPower = failureEffects.Length > 0 && failureEffects.Any(ActionEffectUsesPower);
            GambleSuccess = false;
        }
    }

    protected override bool IsValidTargetInternal(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        foreach (var effect in successEffects)
        {
            if (effect.IsValidTarget(grid, action, sub, user, target, targetData))
                return true;
        }
        if (allowFailureEffectsForValidTargetPosChecks)
        {
            foreach (var effect in failureEffects)
            {
                if (effect.IsValidTarget(grid, action, sub, user, target, targetData))
                    return true;
            }
        }
        return false;
    }

    public static bool ActionEffectUsesPower(ActionEffect e) => e.UsesPower;

    public int ActionMacroDamage(BattleGrid grid, Action action, SubAction sub, Unit user, Queue<int> indices)
    {
        if(indices.Count <= 0)
        {
            return ActionMacroDamage(grid, action, sub, user, true, indices);
        }
        bool isSuccess = indices.Dequeue() == 0;
        return ActionMacroDamage(grid, action, sub, user, isSuccess, indices);
    }

    private int ActionMacroDamage(BattleGrid grid, Action action, SubAction sub, Unit user, bool success, Queue<int> indices)
    {
        var actionEffects = success ? successEffects : failureEffects;
        int effectIndex = indices.Count > 1 ? indices.Dequeue() : 0;
        if (effectIndex >= actionEffects.Length)
            return 0;
        if (actionEffects[effectIndex] is IDamagingActionEffect damageEffect)
            return damageEffect.ActionMacroDamage(grid, action, sub, user, indices);
        return 0;
    }

    public int CalculateDamage(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData, bool simulation)
    {
        return 0; //unable to simulate
    }
}
