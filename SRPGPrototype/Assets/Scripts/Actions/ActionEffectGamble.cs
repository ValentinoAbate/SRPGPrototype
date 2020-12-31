using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandomUtils;
using System.Linq;

public class ActionEffectGamble : ActionEffect
{
    public override bool UsesPower => usedPower;
    private bool usedPower = false;
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
}
