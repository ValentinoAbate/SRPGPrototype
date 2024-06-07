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

    // Sub-action data
    public Type Subtype => subtype;
    [SerializeField] private Type subtype = Type.None;
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

    public void Use(BattleGrid grid, Action action, Unit user, Vector2Int selectedPos)
    {
        // Get target positions
        var targetPositions = targetPattern.Target(grid, user, selectedPos).Where(grid.IsLegal).ToList();
        var targets = new List<Unit>(targetPositions.Count);
        foreach (var position in targetPositions)
        {
            var target = grid.Get(position);
            if (target != null)
                targets.Add(target);
        }
        foreach (var effect in effects)
        {
            effect.Initialize(grid, action, this, user, targetPositions);
            if (effect.AffectUser)
            {
                effect.ApplyEffect(grid, action, this, user, user, new ActionEffect.PositionData(user.Pos, selectedPos));
            }
            else
            {
                foreach(var target in targets)
                {
                    effect.ApplyEffect(grid, action, this, user, target, new ActionEffect.PositionData(target.Pos, selectedPos));
                }
            }
        }
        user.OnAfterSubActionFn?.Invoke(grid, action, this, user, targets, targetPositions);
    }

    public int BaseDamage(Action action, Unit user, params int[] indices)
    {
        if (indices.Length <= 0)
        {
            return effects[0] is IDamagingActionEffect d1 ? d1.BaseDamage(action, this, user) : 0;
        }
        int index = indices[0];
        if (index >= effects.Length)
            return 0;
        if(indices.Length > 1)
        {
            return effects[index] is IDamagingActionEffect d2 ? d2.BaseDamage(action, this, user, indices.Skip(1).ToArray()) : 0;
        }
        return effects[index] is IDamagingActionEffect d3 ? d3.BaseDamage(action, this, user) : 0;
    }
}
