using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBehaviorUseActionOnUnitDamaged : UnitBehavior
{
    [SerializeField] private AIUnit self;
    [SerializeField] private Action action;
    [SerializeField] private bool targetEmptySpaces;
    [SerializeField] private List<Unit.Team> damagedTeams;
    [SerializeField] private List<Unit.Team> targetTeams;

    void Start()
    {
        action = action.Validate(self.ActionTransform);
        if(EncounterEventManager.main != null)
        {
            EncounterEventManager.main.OnUnitDamaged -= OnUnitDamaged;
            EncounterEventManager.main.OnUnitDamaged += OnUnitDamaged;
        }    
    }

    private void OnDestroy()
    {
        Cleanup();
    }

    private void Cleanup()
    {
        if (EncounterEventManager.main != null)
        {
            EncounterEventManager.main.OnUnitDamaged -= OnUnitDamaged;
        }
    }

    private void OnUnitDamaged(BattleGrid grid, Unit damaged, Unit source, int amount)
    {
        if (self.Dead)
        {
            Cleanup();
            return;
        }
        if (amount <= 0 || !damagedTeams.Contains(damaged.UnitTeam))
            return;
        // If action targets self, end early
        if (action.SubActions[0].targetPattern.patternType == TargetPattern.Type.Self)
        {
            action.UseAll(grid, self, self.Pos, false);
        }
        else if (targetEmptySpaces)
        {
            var tPos = AIComponent<AIUnit>.ChooseRandomEmptyTargetPosition(grid, self, action);
            if(tPos != BattleGrid.OutOfBounds)
            {
                action.UseAll(grid, self, tPos, false);
            }
        }
        else
        {
            var targets = grid.FindAll(IsUnitTarget);
            if (targets.Count <= 0)
                return;
            var tPos = AIComponent<AIUnit>.GetFirstValidTargetPosInRange(grid, self, action, targets);
            if (tPos == BattleGrid.OutOfBounds)
                return;
            action.UseAll(grid, self, tPos, false);
        }
    }

    private bool IsUnitTarget(Unit u) => targetTeams.Contains(u.UnitTeam);
}
