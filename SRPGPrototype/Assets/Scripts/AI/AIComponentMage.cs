using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIComponentMage : AIComponent<AIUnit>
{
    private enum ReadyOption
    {
        Standard,
        StartReady,
        ReadyEveryTurn,
    }
    [SerializeField] private ReadyOption readyOption;
    protected virtual Action StandardAction => standardAction;
    [SerializeField] private Action standardAction;
    protected virtual Action MoveAction => moveAction;
    [SerializeField] private Action moveAction;
    [SerializeField] private Animator animator;
    [SerializeField] private List<Unit.Team> targetTeams = new List<Unit.Team> { Unit.Team.Player };
    [SerializeField] private List<Unit.Team> runAwayFromTeams = new List<Unit.Team> { Unit.Team.Player };
    private static readonly int readyHash = Animator.StringToHash("Ready");

    public override List<Action> Actions => new List<Action> { StandardAction, MoveAction };

    private bool readyToCast = false;

    public override void Initialize(AIUnit self)
    {
        standardAction = standardAction.Validate(self.ActionTransform);
        moveAction = moveAction.Validate(self.ActionTransform);
        if(readyOption == ReadyOption.ReadyEveryTurn || readyOption == ReadyOption.StartReady)
        {
            SetCastReady(true);
        }
    }

    public override IEnumerator DoTurn(BattleGrid grid, AIUnit self)
    {
        // Casting
        if (readyToCast)
        {
            var subAction = StandardAction.SubActions[0];
            SetCastReady(false);
            // If action targets self, end early
            if (subAction.targetPattern.patternType == TargetPattern.Type.Self)
            {
                yield return StartCoroutine(AttackUntilExhausted(grid, self, StandardAction, self.Pos));
            }
            else
            {
                var attackRoutine = AttackFirstUnitInRange(grid, self, standardAction, IsUnitTarget);
                if(attackRoutine != null)
                {
                    yield return attackRoutine;
                }
            }
            if(readyOption == ReadyOption.ReadyEveryTurn)
            {
                SetCastReady(true);
            }
        }
        else
        {
            SetCastReady(true);
        }
        var runRoutine = RunAway(grid, self, moveAction, RunAwayFromUnit);
        if(runRoutine != null)
        {
            yield return runRoutine;
        }

    }

    bool IsUnitTarget(Unit other) => targetTeams.Contains(other.UnitTeam);
    bool RunAwayFromUnit(Unit other) => runAwayFromTeams.Contains(other.UnitTeam);

    private void SetCastReady(bool value)
    {
        animator.SetBool(readyHash, value);
        readyToCast = value;
    }
}
