using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SubAction : MonoBehaviour
{
    public bool DealsDamage => effects.Any((e) => e is ActionEffectDamage);
    public bool UsesPower => effects.Any((e) => e.UsesPower);
    public bool hasRange = false;
    public bool hasPattern = false;
    public Pattern range;
    public TargetPattern targetPattern;
    private ActionEffect[] effects;

    private void Awake()
    {
        effects = GetComponents<ActionEffect>();
    }

    public void Use(BattleGrid grid, Action action, Unit user, Vector2Int selectedPos)
    {
        // Get target positions
        var targetPositions = hasPattern ? targetPattern.Target(grid, user, selectedPos) : new List<Vector2Int>();
        // Remove all illegal target positions
        targetPositions.RemoveAll((p) => !grid.IsLegal(p));
        var targets = new List<Unit>();
        foreach (var effect in effects)
        {
            effect.Initialize(grid, action, user, targetPositions);
            foreach(var position in targetPositions)
            {
                var target = grid.Get(position);
                if (target != null)
                    targets.Add(target);
                effect.ApplyEffect(grid, action, user, target, new ActionEffect.PositionData(position, selectedPos));
            }
        }
        user.OnAfterSubActionFn?.Invoke(grid, action, this, user, targets, targetPositions);
    }
}
