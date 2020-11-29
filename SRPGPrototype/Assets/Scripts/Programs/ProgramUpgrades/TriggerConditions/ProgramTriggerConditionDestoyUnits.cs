using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProgramTriggerConditionDestoyUnits : ProgramTriggerCondition
{
    public override bool Completed => progress >= number;

    [SerializeField] private Unit.Team[] teams = new Unit.Team[] { Unit.Team.Enemy };

    [SerializeField] private int number = 5;

    private int progress = 0;

    private readonly List<Action> actions = new List<Action>();

    public override string RevealedConditionText => "Destroy " + number + " " + string.Join("/",teams) + " units with this program (" + progress + "/" + number + ")";
    public override void LinkEffect(Program program, ref Shell.CompileData data)
    {
        actions.Clear();
        // Log actions from the program
        actions.AddRange(program.Effects.Where((e) => e is ProgramEffectAddAction).Select((e) => (e as ProgramEffectAddAction).action));
        // Add the check
        data.abilityOnAfterSubAction += Check;
    }
    private void Check(BattleGrid grid, Action action, SubAction subAction, Unit user, List<Unit> targets, List<Vector2Int> targetPositions)
    {
        if (!actions.Contains(action))
            return;
        progress += targets.Where((t) => t.Dead && teams.Contains(t.UnitTeam)).Count();
    }
}
