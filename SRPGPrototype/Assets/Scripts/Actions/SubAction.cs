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
    public bool DealsDamage => effects.Any((e) => e.DealsDamage);
    public bool UsesPower => effects.Any((e) => e.UsesPower);

    // Sub-action data
    public Type Subtype => subtype;
    [SerializeField] private Type subtype = Type.None;
    public RangePattern Range => rangePattern;
    [SerializeField]  private RangePattern rangePattern = new RangePattern();
    public TargetPattern targetPattern;
    private ActionEffect[] effects;

    private void Awake()
    {
        effects = GetComponents<ActionEffect>();
    }

    public void Use(BattleGrid grid, Action action, Unit user, Vector2Int selectedPos)
    {
        // Get target positions
        var targetPositions = targetPattern.Target(grid, user, selectedPos).Where(grid.IsLegal).ToList();
        var targets = new List<Unit>();
        foreach (var effect in effects)
        {
            effect.Initialize(grid, action, this, user, targetPositions);
            if(effect.AffectUser)
            {
                effect.ApplyEffect(grid, action, this, user, user, new ActionEffect.PositionData(user.Pos, selectedPos));
            }
            else
            {
                foreach (var position in targetPositions)
                {
                    var target = grid.Get(position);
                    if (target != null)
                        targets.Add(target);
                    effect.ApplyEffect(grid, action, this, user, target, new ActionEffect.PositionData(position, selectedPos));
                }
            }
        }
        user.OnAfterSubActionFn?.Invoke(grid, action, this, user, targets, targetPositions);
    }
}
