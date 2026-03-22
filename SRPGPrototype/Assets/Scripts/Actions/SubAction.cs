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
        ApplySubtypeToSelf = 8,
        DontApplySubtypeToTargets = 16,
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
    public RangePattern.Type RangeType => rangePattern.patternType;
    public RangePatternGenerator RangeGenerator => rangePattern.generator;
    public int MaxRange(BattleGrid grid) => rangePattern.MaxDistance(grid);
    [SerializeField]  private RangePattern rangePattern = new RangePattern();
    public TargetPattern.Type TargetType => targetPattern.patternType;
    public TargetPatternGenerator TargetPatternGenerator => targetPattern.generator;
    [SerializeField] private TargetPattern targetPattern;
    public IReadOnlyList<ActionEffect> Effects => effects;
    [SerializeField] private ActionEffect[] effects;

    private void Awake()
    {
        if(effects == null || effects.Length <= 0)
        {
            effects = GetComponents<ActionEffect>();
        }
    }

    public IEnumerable<Vector2Int> Target(BattleGrid grid, Action action, Vector2Int selectedPos, Unit user)
    {
        if(action.Program == null)
        {
            foreach(var pos in targetPattern.Target(grid, user, selectedPos))
            {
                if (grid.IsLegal(pos))
                    yield return pos;
            }
            yield break;
        }
        foreach (var pos in targetPattern.TargetWithMods(action.Program.ModifiedByTypeSubAction<ModifierTargetPattern>(this), grid, user, selectedPos, rangePattern))
        {
            if (grid.IsLegal(pos))
                yield return pos;
        }
    }

    public IEnumerable<Vector2Int> ReverseTarget(BattleGrid grid, Vector2Int targetPos)
    {
        return targetPattern.ReverseTarget(grid, targetPos); // TODO: mod compat
    }

    public IEnumerable<Vector2Int> Range(BattleGrid grid, Action action, Vector2Int origin, Unit user)
    {
        if (action.Program == null)
        {
            foreach (var pos in rangePattern.GetPositions(grid, origin, user))
            {
                if (grid.IsLegal(pos))
                    yield return pos;
            }
            yield break;
        }
        foreach (var pos in rangePattern.GetPositionsWithMods(action.Program.ModifiedByTypeSubAction<ModifierRange>(this), grid, origin, user))
        {
            if (grid.IsLegal(pos))
                yield return pos;
        }
    }

    public IEnumerable<Vector2Int> ReverseRange(BattleGrid grid, Vector2Int targetPos, Unit user)
    {
        return rangePattern.ReverseRange(grid, targetPos, user); // TODO: mod compat
    }

    public IEnumerable<Vector2Int> GetValidRangePositions(BattleGrid grid, Action action, Vector2Int origin, Unit user, IReadOnlyList<Unit> lastTargets)
    {
        var targetPositions = new List<Vector2Int>();
        var targets = new List<Unit>();
        var emptyTargetPositions = new List<Vector2Int>();
        foreach(var pos in Range(grid, action, origin, user))
        {
            GetTargetsAndTargetPositions(grid, action, user, pos, lastTargets, ref targetPositions, ref targets, ref emptyTargetPositions);
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
            return effect.IsValidTarget(grid, action, this, user, user, new ActionEffect.PositionData(user.Pos, selectedPos, user.Pos));
        }
        foreach (var pos in emptyTargetPositions)
        {
            if (effect.IsValidTarget(grid, action, this, user, null, new ActionEffect.PositionData(pos, selectedPos, user.Pos)))
                return true;
        }
        foreach (var target in targets)
        {
            if (effect.IsValidTarget(grid, action, this, user, target, new ActionEffect.PositionData(target.Pos, selectedPos, user.Pos)))
                return true;
        }
        return false;
    }

    private void GetTargetsAndTargetPositions(BattleGrid grid, Action action, Unit user, Vector2Int selectedPos, IReadOnlyList<Unit> lastTargets, ref List<Vector2Int> targetPositions, ref List<Unit> targets, ref List<Vector2Int> emptyTargetPositions)
    {
        // Get target positions
        targetPositions.Clear();
        targetPositions.AddRange(Target(grid, action, selectedPos, user));
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
                if (grid.TryGet(position, out var target))
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
        GetTargetsAndTargetPositions(grid, action, user, selectedPos, lastTargets, ref targetPositions, ref targets, ref emptyTargetPositions);
        if (effects.Length <= 0)
            return;
        List<Unit> userList = null;
        if(subtype == Type.OverrideByEffects)
        {
            var originalPos = user.Pos;
            foreach (var effect in effects)
            {
                if(effect.StandaloneSubActionType != Type.None)
                {
                    OnBeforeSubAction(grid, action, user, targets, ref userList, targetPositions, effect.SubActionOptions, effect.StandaloneSubActionType);
                    ApplyEffect(effect, grid, action, user, selectedPos, originalPos, targets, emptyTargetPositions, targetPositions);
                    OnAfterSubAction(grid, action, user, targets, ref userList, targetPositions, effect.SubActionOptions, effect.StandaloneSubActionType);
                }
                else
                {
                    ApplyEffect(effect, grid, action, user, selectedPos, originalPos, targets, emptyTargetPositions, targetPositions);
                }
            }
        }
        else
        {
            var originalPos = user.Pos;
            OnBeforeSubAction(grid, action, user, targets, ref userList, targetPositions, options);
            foreach (var effect in effects)
            {
                ApplyEffect(effect, grid, action, user, selectedPos, originalPos, targets, emptyTargetPositions, targetPositions);
            }
            OnAfterSubAction(grid, action, user, targets, ref userList, targetPositions, options);
        }
        if(rangePattern.patternType == RangePattern.Type.Generated)
        {
            rangePattern.generator.OnUsed();
        }
    }

    private void OnBeforeSubAction(BattleGrid grid, Action action, Unit user, List<Unit> targets, ref List<Unit> userList, List<Vector2Int> targetPositions, Options options, Type subTypeOverride = Type.None)
    {
        if (!options.HasFlag(Options.DontApplySubtypeToTargets))
        {
            user.OnBeforeSubActionFn?.Invoke(grid, action, this, user, targets, targetPositions, options, subTypeOverride);
        }
        if (options.HasFlag(Options.ApplySubtypeToSelf))
        {
            user.OnBeforeSubActionFn?.Invoke(grid, action, this, user, userList ??= new List<Unit>() { user }, targetPositions, options, subTypeOverride);
        }
    }

    private void OnAfterSubAction(BattleGrid grid, Action action, Unit user, List<Unit> targets, ref List<Unit> userList, List<Vector2Int> targetPositions, Options options, Type subTypeOverride = Type.None)
    {
        if (!options.HasFlag(Options.DontApplySubtypeToTargets))
        {
            user.OnAfterSubActionFn?.Invoke(grid, action, this, user, targets, targetPositions, options, subTypeOverride);
        }
        if (options.HasFlag(Options.ApplySubtypeToSelf))
        {
            user.OnAfterSubActionFn?.Invoke(grid, action, this, user, userList ??= new List<Unit>() { user }, targetPositions, options, subTypeOverride);
        }
    }

    private void ApplyEffect(ActionEffect effect, BattleGrid grid, Action action, Unit user, Vector2Int selectedPos, Vector2Int originalUserPos, List<Unit> targets, List<Vector2Int> emptyTargetPositions, List<Vector2Int> targetPositions)
    {
        effect.Initialize(grid, action, this, user, targetPositions);
        if (effect.AffectUser)
        {
            effect.ApplyEffect(grid, action, this, user, user, new ActionEffect.PositionData(user.Pos, selectedPos, originalUserPos));
            return;
        }
        foreach (var pos in emptyTargetPositions)
        {
            effect.ApplyEffect(grid, action, this, user, null, new ActionEffect.PositionData(pos, selectedPos, originalUserPos));
        }
        foreach (var target in targets)
        {
            effect.ApplyEffect(grid, action, this, user, target, new ActionEffect.PositionData(target.Pos, selectedPos, originalUserPos));
        }
    }

    public int ActionMacroDamage(BattleGrid grid, Action action, Unit user, Queue<int> indices)
    {
        int effectIndex = indices.Count > 0 ? indices.Dequeue() : 0;
        if (effectIndex >= effects.Length)
            return 0;
        if (effects[effectIndex] is IDamagingActionEffect damageEffect)
        {
            int baseDamage = damageEffect.ActionMacroDamage(grid, action, this, user, indices);
            if(user != null && damageEffect.UsesPower)
            {
                baseDamage += user.Power;
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

    private const char separator = '-';

    public bool CanSave(bool isBattle)
    {
        if (rangePattern.patternType == RangePattern.Type.Generated && rangePattern.generator.CanSave(isBattle))
            return true;
        foreach(var effect in effects)
        {
            if (effect.CanSave(isBattle))
                return true;
        }
        return false;
    }

    public string Save(bool isBattle)
    {
        System.Text.StringBuilder builder = null;
        if (rangePattern.patternType == RangePattern.Type.Generated && rangePattern.generator.CanSave(isBattle))
        {
            builder ??= new System.Text.StringBuilder();
            builder.Append(rangePattern.generator.Save(isBattle));
            builder.Append(separator);
        }
        foreach (var effect in effects)
        {
            if (effect.CanSave(isBattle))
            {
                builder ??= new System.Text.StringBuilder();
                builder.Append(effect.Save(isBattle));
                builder.Append(separator);
            }
        }
        if (builder == null)
            return string.Empty;
        builder.Remove(builder.Length - 1, 1);
        return builder.ToString();
    }

    public void Load(string data, bool isBattle)
    {
        int effectInd = 0;
        int argInd = 0;
        var args = data.Split(separator);
        if (rangePattern.patternType == RangePattern.Type.Generated && rangePattern.generator.CanSave(isBattle))
        {
            rangePattern.generator.Load(args[argInd++], isBattle);
        }
        while (effectInd < effects.Length && argInd < args.Length)
        {
            var effect = effects[effectInd++];
            if (effect.CanSave(isBattle))
            {
                effect.Load(args[argInd++], isBattle);
            }
        }
    }
}
