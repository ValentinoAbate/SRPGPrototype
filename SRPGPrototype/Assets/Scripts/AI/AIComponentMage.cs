using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIComponentMage : AIComponent
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
    [SerializeField] private bool targetEmptySpaces;
    [SerializeField] private Action attackAction;
    [SerializeField] private Action moveAction;
    
    [SerializeField] private Animator animator;
    [SerializeField] private List<Unit.Team> targetTeams = new List<Unit.Team> { Unit.Team.Player };
    [SerializeField] private List<Unit.Team> runAwayFromTeams = new List<Unit.Team> { Unit.Team.Player };
    private static readonly int readyHash = Animator.StringToHash("Ready");

    public override IEnumerable<Action> Actions
    {
        get
        {
            yield return StandardAction;
            yield return moveAction;
            if(attackAction != null)
            {
                yield return attackAction;
            }
        }
    }

    private bool readyToCast = false;

    public override void Initialize(AIUnit self)
    {
        standardAction = standardAction.Validate(self.ActionTransform);
        moveAction = moveAction.Validate(self.ActionTransform);
        if(attackAction != null)
        {
            attackAction = attackAction.Validate(self.ActionTransform);
        }
        if(readyOption == ReadyOption.ReadyEveryTurn || readyOption == ReadyOption.StartReady)
        {
            SetCastReady(true);
        }
    }

    public override IEnumerator DoTurn(BattleGrid grid, AIUnit self)
    {
        // Attack if in range
        if(attackAction != null)
        {
            var attackRoutine = AttackBestPositionInRange(grid, self, attackAction, UnitFilters.IsPlayer);
            if (attackRoutine != null)
            {
                yield return attackRoutine;
            }
        }
        // Casting
        if (readyToCast)
        {

            // If action targets self, end early
            if (StandardAction.SubActions[0].targetPattern.patternType == TargetPattern.Type.Self)
            {
                SetCastReady(false);
                yield return StartCoroutine(AttackUntilExhausted(grid, self, StandardAction, self.Pos));
                CheckReadyEveryTurn();
            }
            else if (targetEmptySpaces)
            {
                // Check for targetspace in range
                while (!self.Dead && self.CanUseAction(StandardAction))
                {
                    var tPos = ChooseRandomEmptyTargetPosition(grid, self, StandardAction);
                    if (tPos == BattleGrid.OutOfBounds)
                        break;
                    SetCastReady(false);
                    StandardAction.UseAll(grid, self, tPos);
                    yield return attackDelay;
                }
                CheckReadyEveryTurn();
            }
            else
            {
                var attackRoutine = AttackFirstUnitInRange(grid, self, StandardAction, IsUnitTarget);
                if (attackRoutine != null)
                {
                    SetCastReady(false);
                    yield return attackRoutine;
                    CheckReadyEveryTurn();
                }
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

    private void CheckReadyEveryTurn()
    {
        if (readyOption == ReadyOption.ReadyEveryTurn)
        {
            SetCastReady(true);
        }
    }
}
