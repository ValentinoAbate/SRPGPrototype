using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SubAction : MonoBehaviour
{
    public enum Type
    { 
        None,
        Special,
        // Move subtypes
        Move,
        Jump,
        Warp,
        // Skill subtypes
        Buff,
        Summon,
        Reposition,
        // Weapon subtypes
        Umbrella,
        Sword,
        Spear,
        Hammer,
        Gun,
        Explosive,
        Spell,
        OverrideByEffects,
    }

    [System.Flags]
    public enum Options
    {
        None = 0,
        SkipUpgradeCheck = 1,
        ApplyToLastTargets = 2,
        RangeBasedOnLastSelectorPos = 4,
    }

    // Sub-action Metadata
    public bool DealsDamage
    {
        get
        {
            foreach(var effect in effects)
            {
                if(effect is IDamagingActionEffect damageEffect && damageEffect.DealsDamage)
                {
                    return true;
                }
            }
            return false;
        }
    }
    public bool UsesPower => effects.Any((e) => e.UsesPower);
    public bool UsedPower { get; private set; } = false;

    // Sub-action data
    public string SubTypeText
    {
        get
        {
            if(subtype == Type.OverrideByEffects)
            {
                var texts = new List<string>(effects.Length);
                foreach(var effect in effects)
                {
                    if (effect.StandaloneSubActionType == Type.None)
                        continue;
                    texts.Add(effect.StandaloneSubActionType.ToString());
                }
                return string.Join(" / ", texts);
            }
            return subtype == Type.None ? string.Empty : subtype.ToString();
        }
    }
    [SerializeField] private Type subtype = Type.None;
    public Options OptionFlags => options;
    [SerializeField] private Options options = Options.None;
    public RangePattern Range => rangePattern;
    [SerializeField]  private RangePattern rangePattern = new RangePattern();
    public TargetPattern targetPattern;
    public IReadOnlyList<ActionEffect> Effects => effects;
    [SerializeField] private ActionEffect[] effects;

    private void Awake()
    {
        if(effects == null || effects.Length <= 0)
        {
            effects = GetComponents<ActionEffect>();
        }
    }

    public void Initialize()
    {
        UsedPower = false;
    }

    public void Use(BattleGrid grid, Action action, Unit user, Vector2Int selectedPos, IReadOnlyList<Unit> lastTargets, out List<Unit> targets)
    {
        // Get target positions
        var targetPositions = targetPattern.Target(grid, user, selectedPos).Where(grid.IsLegal).ToList();
        var emptyTargetPositions = new List<Vector2Int>(targetPositions.Count);
        if (options.HasFlag(Options.ApplyToLastTargets))
        {
            targets = new List<Unit>(lastTargets);
        }
        else
        {
            targets = new List<Unit>(targetPositions.Count);
            foreach (var position in targetPositions)
            {
                var target = grid.Get(position);
                if (target != null)
                {
                    targets.Add(target);
                }
                else
                {
                    emptyTargetPositions.Add(position);
                }
            }
        }
        if (effects.Length <= 0)
            return;
        if(subtype == Type.OverrideByEffects)
        {
            foreach (var effect in effects)
            {
                if(effect.StandaloneSubActionType != Type.None)
                {
                    user.OnBeforeSubActionFn?.Invoke(grid, action, this, user, targets, targetPositions, effect.SubActionOptions, effect.StandaloneSubActionType);
                    UsedPower |= effect.UsesPower && !user.Power.IsZero;
                    ApplyEffect(effect, grid, action, user, selectedPos, targets, emptyTargetPositions, targetPositions);
                    user.OnAfterSubActionFn?.Invoke(grid, action, this, user, targets, targetPositions, effect.SubActionOptions, effect.StandaloneSubActionType);
                }
                else
                {
                    UsedPower |= effect.UsesPower && !user.Power.IsZero;
                    ApplyEffect(effect, grid, action, user, selectedPos, targets, emptyTargetPositions, targetPositions);
                }
            }
        }
        else
        {
            user.OnBeforeSubActionFn?.Invoke(grid, action, this, user, targets, targetPositions, options);
            UsedPower = UsesPower && !user.Power.IsZero;
            foreach (var effect in effects)
            {
                ApplyEffect(effect, grid, action, user, selectedPos, targets, emptyTargetPositions, targetPositions);
            }
            user.OnAfterSubActionFn?.Invoke(grid, action, this, user, targets, targetPositions, options);
        }
    }

    private void ApplyEffect(ActionEffect effect, BattleGrid grid, Action action, Unit user, Vector2Int selectedPos, List<Unit> targets, List<Vector2Int> emptyTargetPositions, List<Vector2Int> targetPositions)
    {
        effect.Initialize(grid, action, this, user, targetPositions);
        if (effect.AffectUser)
        {
            effect.ApplyEffect(grid, action, this, user, user, new ActionEffect.PositionData(user.Pos, selectedPos));
            return;
        }
        foreach (var pos in emptyTargetPositions)
        {
            effect.ApplyEffect(grid, action, this, user, null, new ActionEffect.PositionData(pos, selectedPos));
        }
        foreach (var target in targets)
        {
            effect.ApplyEffect(grid, action, this, user, target, new ActionEffect.PositionData(target.Pos, selectedPos));
        }

    }

    public int BaseDamage(Action action, Unit user, Queue<int> indices)
    {
        int effectIndex = indices.Count > 0 ? indices.Dequeue() : 0;
        if (effectIndex >= effects.Length)
            return 0;
        if (effects[effectIndex] is IDamagingActionEffect damageEffect)
        {
            int baseDamage = damageEffect.BaseDamage(action, this, user, indices);
            if(user != null && damageEffect.UsesPower)
            {
                baseDamage += user.Power.Value;
            }
            return baseDamage;
        }
        return 0;
    }

    public bool HasSubType(Type subT)
    {
        if(subtype != Type.OverrideByEffects)
        {
            return subtype == subT;
        }
        foreach(var effect in effects)
        {
            if (effect.StandaloneSubActionType == subT)
                return true;
        }
        return false;
    }

    public bool HasAnySubType(params Type[] subTypes)
    {
        foreach(var subT in subTypes)
        {
            if (HasSubType(subT))
                return true;
        }
        return false;
    }
}
