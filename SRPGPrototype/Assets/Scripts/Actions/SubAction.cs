﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubAction : MonoBehaviour
{
    public bool hasRange = false;
    public bool hasPattern = false;
    public Pattern range;
    public TargetPattern targetPattern;
    private ActionEffect[] effects;

    private void Awake()
    {
        effects = GetComponents<ActionEffect>();
    }

    public void Use(BattleGrid grid, Combatant user, Vector2Int selectedPos)
    {
        // Get target positions
        var targetPositions = hasPattern ? targetPattern.Target(grid, user, selectedPos) : new List<Vector2Int>();
        // Remove all illegal target positions
        targetPositions.RemoveAll((p) => !grid.IsLegal(p));

        foreach (var effect in effects)
        {
            effect.Initialize(grid, user, targetPositions);
            foreach(var position in targetPositions)
            {
                effect.ApplyEffect(grid, user, new ActionEffect.PositionData(position, selectedPos));
            }
        }
    }
}
