using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBehaviorUseActionOnUnitDamaged : UnitBehavior
{
    [SerializeField] private Action action;
    [SerializeField] private bool targetEmptySpaces;
    [SerializeField] private List<Unit.Team> damagedTeams;
    [SerializeField] private List<Unit.Team> targetTeams;

    public bool TriggersOnBleed => damagedTeams.Contains(Unit.Team.Player);

    void Start()
    {
        action = action.Validate(self.ActionTransform); 
    }

    protected override void AttachListeners()
    {
        if (EncounterEventManager.main != null)
        {
            EncounterEventManager.main.OnUnitDamaged -= OnUnitDamaged;
            EncounterEventManager.main.OnUnitDamaged += OnUnitDamaged;
        }
    }

    protected override void CleanupListeners()
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
            CleanupListeners();
            return;
        }
        if (amount <= 0 || !damagedTeams.Contains(damaged.UnitTeam))
            return;
        Trigger(grid);
    }

    public void Trigger(BattleGrid grid)
    {
        // If action targets self, end early
        if (action.SubActions[0].TargetType == TargetPattern.Type.Self)
        {
            action.UseAll(grid, self, self.Pos, false);
        }
        else if (targetEmptySpaces)
        {
            var tPos = AIComponent.ChooseRandomEmptyTargetPosition(grid, self, action);
            if (tPos != BattleGrid.OutOfBounds)
            {
                action.UseAll(grid, self, tPos, false);
            }
        }
        else
        {
            var targets = grid.FindAll(IsUnitTarget);
            if (targets.Count <= 0)
                return;
            var tPos = AIComponent.GetFirstValidTargetPosInRange(grid, self, action, targets);
            if (tPos == BattleGrid.OutOfBounds)
                return;
            action.UseAll(grid, self, tPos, false);
        }
    }

    private bool IsUnitTarget(Unit u) => targetTeams.Contains(u.UnitTeam);
}
