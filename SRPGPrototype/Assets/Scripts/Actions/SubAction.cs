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
        AlsoApplySubtypeToSelf = 8,
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

    public IEnumerable<Vector2Int> GetValidRangePositions(BattleGrid grid, Action action, Vector2Int origin, Unit user, IReadOnlyList<Unit> lastTargets)
    {
        var targetPositions = new List<Vector2Int>();
        var targets = new List<Unit>();
        var emptyTargetPositions = new List<Vector2Int>();
        foreach(var pos in Range.GetPositions(grid, origin, user))
        {
            GetTargetsAndTargetPositions(grid, user, pos, lastTargets, ref targetPositions, ref targets, ref emptyTargetPositions);
            foreach(var effect in effects)
            {
                if (effect.IgnoreInValidRangeCalcs)
                    continue;
                if(IsValidRangePosForEffect(effect, grid, action, user, pos, targets, emptyTargetPositions, targetPositions))
                {
                    yield return pos;
                    break;
                }
            }
        }
    }

    private bool IsValidRangePosForEffect(ActionEffect effect, BattleGrid grid, Action action, Unit user, Vector2Int selectedPos, List<Unit> targets, List<Vector2Int> emptyTargetPositions, List<Vector2Int> targetPositions)
    {
        if (effect.AffectUser)
        {
            return effect.IsValidTarget(grid, action, this, user, user, new ActionEffect.PositionData(user.Pos, selectedPos));
        }
        foreach (var pos in emptyTargetPositions)
        {
            if (effect.IsValidTarget(grid, action, this, user, null, new ActionEffect.PositionData(pos, selectedPos)))
                return true;
        }
        foreach (var target in targets)
        {
            if (effect.IsValidTarget(grid, action, this, user, target, new ActionEffect.PositionData(target.Pos, selectedPos)))
                return true;
        }
        return false;
    }

    private void GetTargetsAndTargetPositions(BattleGrid grid, Unit user, Vector2Int selectedPos, IReadOnlyList<Unit> lastTargets, ref List<Vector2Int> targetPositions, ref List<Unit> targets, ref List<Vector2Int> emptyTargetPositions)
    {
        // Get target positions
        targetPositions.Clear();
        targetPositions.AddRange(targetPattern.Target(grid, user, selectedPos).Where(grid.IsLegal));
        // Prepare target lists
        emptyTargetPositions.Clear();
        targets.Clear();
        if (options.HasFlag(Options.ApplyToLastTargets))
        {
            targets.AddRange(lastTargets);
        }
        else
        {
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
    }

    public void Use(BattleGrid grid, Action action, Unit user, Vector2Int selectedPos, IReadOnlyList<Unit> lastTargets, out List<Unit> targets)
    {
        var targetPositions = new List<Vector2Int>();
        var emptyTargetPositions = new List<Vector2Int>();
        targets = new List<Unit>();
        GetTargetsAndTargetPositions(grid, user, selectedPos, lastTargets, ref targetPositions, ref targets, ref emptyTargetPositions);
        if (effects.Length <= 0)
            return;
        List<Unit> userList = null;
        if(subtype == Type.OverrideByEffects)
        {
            foreach (var effect in effects)
            {
                if(effect.StandaloneSubActionType != Type.None)
                {
                    OnBeforeSubAction(grid, action, user, targets, ref userList, targetPositions, effect.SubActionOptions, effect.StandaloneSubActionType);
                    UsedPower |= effect.UsesPower && !user.Power.IsZero;
                    ApplyEffect(effect, grid, action, user, selectedPos, targets, emptyTargetPositions, targetPositions);
                    OnAfterSubAction(grid, action, user, targets, ref userList, targetPositions, effect.SubActionOptions, effect.StandaloneSubActionType);
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
            OnBeforeSubAction(grid, action, user, targets, ref userList, targetPositions, options);
            UsedPower = UsesPower && !user.Power.IsZero;
            foreach (var effect in effects)
            {
                ApplyEffect(effect, grid, action, user, selectedPos, targets, emptyTargetPositions, targetPositions);
            }
            OnAfterSubAction(grid, action, user, targets, ref userList, targetPositions, options);
        }
    }

    private void OnBeforeSubAction(BattleGrid grid, Action action, Unit user, List<Unit> targets, ref List<Unit> userList, List<Vector2Int> targetPositions, Options options, Type subTypeOverride = Type.None)
    {
        user.OnBeforeSubActionFn?.Invoke(grid, action, this, user, targets, targetPositions, options, subTypeOverride);
        if (options.HasFlag(Options.AlsoApplySubtypeToSelf))
        {
            user.OnBeforeSubActionFn?.Invoke(grid, action, this, user, userList ??= new List<Unit>() { user }, targetPositions, options, subTypeOverride);
        }
    }

    private void OnAfterSubAction(BattleGrid grid, Action action, Unit user, List<Unit> targets, ref List<Unit> userList, List<Vector2Int> targetPositions, Options options, Type subTypeOverride = Type.None)
    {
        user.OnAfterSubActionFn?.Invoke(grid, action, this, user, targets, targetPositions, options, subTypeOverride);
        if (options.HasFlag(Options.AlsoApplySubtypeToSelf))
        {
            user.OnAfterSubActionFn?.Invoke(grid, action, this, user, userList ??= new List<Unit>() { user }, targetPositions, options, subTypeOverride);
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

    public int ActionMacroDamage(Action action, Unit user, Queue<int> indices)
    {
        int effectIndex = indices.Count > 0 ? indices.Dequeue() : 0;
        if (effectIndex >= effects.Length)
            return 0;
        if (effects[effectIndex] is IDamagingActionEffect damageEffect)
        {
            int baseDamage = damageEffect.ActionMacroDamage(action, this, user, indices);
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

    public bool HasAnySubType(IEnumerable<Type> subTypes)
    {
        foreach(var subT in subTypes)
        {
            if (HasSubType(subT))
                return true;
        }
        return false;
    }

    // If no subtypes are passed in, returns true
    public bool HasAnySubTypeOptional(ICollection<Type> subTypes)
    {
        return subTypes.Count <= 0 || HasAnySubType(subTypes);
    }
}
