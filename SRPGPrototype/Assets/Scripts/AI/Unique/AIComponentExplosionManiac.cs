using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIComponentExplosionManiac : AIComponent<AIUnit>
{
    public override IEnumerable<Action> Actions
    {
        get
        {
            yield return moveAction;
            yield return detonateAction;
            yield return summonActionInert;
            yield return summonActionStandard;
        }
    }

    [SerializeField] private Action moveAction;
    [SerializeField] private Action detonateAction; // should be able to target all explosives and instakill them
    [SerializeField] private Action summonActionInert;
    [SerializeField] private Action summonActionStandard;

    public override void Initialize(AIUnit self)
    {
        moveAction = moveAction.Validate(self.ActionTransform);
        detonateAction = detonateAction.Validate(self.ActionTransform);
        summonActionInert = summonActionInert.Validate(self.ActionTransform);
        summonActionStandard = summonActionStandard.Validate(self.ActionTransform);
    }

    public override IEnumerator DoTurn(BattleGrid grid, AIUnit self)
    {
        // Try using detonate action (if applicable target is found)
        if (self.CanUseAction(detonateAction))
        {
            var detonateTargets = grid.FindUnitsWithTags(Unit.Tags.Explosive);
            if (detonateTargets.Any())
            {
                Unit bestTarget = null;
                int bestTargetScore = 0;
                var seenUnits = new HashSet<Unit>();
                foreach (var target in detonateTargets)
                {
                    seenUnits.Clear();
                    int score = GetDetonateTargetScore(grid, self, target, ref seenUnits);
                    if (score > bestTargetScore)
                    {
                        bestTargetScore = score;
                        bestTarget = target;
                    }
                }
                if (bestTarget != null)
                {
                    yield return attackDelay;
                    detonateAction.UseAll(grid, self, bestTarget.Pos);
                }
            }
        }


        // Run away
        var runAwayRoutine = RunAway(grid, self, moveAction, IsPlayerUnit, summonActionStandard.APCost);
        if(runAwayRoutine != null)
        {
            yield return runAwayRoutine;
        }

        // Spawn bombs (if applicable)
        if (self.CanUseAction(summonActionStandard))
        {
            bool applyAPCost = true;
            var targetUnits = grid.FindAll(IsPlayerUnit);
            float Weight(Vector2Int pos) => GetSummonPositionScore(grid, self, targetUnits, pos);
            var positionWeights = new WeightedSet<Vector2Int>(summonActionInert.GetRange(grid, self.Pos, self), Weight);
            // Throw object first
            if (positionWeights.Count > 0)
            {
                yield return attackDelay;
                summonActionInert.UseAll(grid, self, RandomU.instance.Choice(positionWeights), applyAPCost);
                applyAPCost = false;
            }
            // Throw units next
            int numUnits = RandomU.instance.RollSuccess((1 - ((double)self.HP / self.MaxHP)) + 0.2) ? 2 : 3;
            for (int i = 0; i < numUnits; i++)
            {
                positionWeights.Clear();
                positionWeights.AddRange(summonActionStandard.GetRange(grid, self.Pos, self), Weight);
                if (positionWeights.Count > 0)
                {
                    yield return attackDelay;
                    summonActionStandard.UseAll(grid, self, RandomU.instance.Choice(positionWeights), applyAPCost);
                    applyAPCost = false;
                }
            }
        }
    }

    private static bool IsPlayerUnit(Unit u) => u.UnitTeam == Unit.Team.Player;

    private float GetSummonPositionScore(BattleGrid grid, AIUnit self, IEnumerable<Unit> targetUnits, Vector2Int pos)
    {
        if (!grid.IsLegalAndEmpty(pos))
            return 0;
        float score = 1;
        // Target Unit Distance Factor
        float bestDistance = 0;
        foreach(var unit in targetUnits)
        {
            float distance = Vector2Int.Distance(unit.Pos, pos);
            if(distance < bestDistance || bestDistance <= 0)
            {
                bestDistance = distance;
            }
        }
        if(bestDistance > 0)
        {
            score *= Mathf.Pow(grid.MaxDistance - bestDistance, 2);
        }
        // Unclumping factor
        foreach(var adjPos in pos.Adjacent())
        {
            var unit = grid.Get(adjPos);
            if(unit != null && unit.UnitTeam == self.UnitTeam)
            {
                score *= 0.25f;
            }
        }
        return score;
    }

    private int GetDetonateTargetScore(BattleGrid grid, AIUnit self, Unit target, ref HashSet<Unit> seenUnits)
    {
        if (seenUnits.Contains(target))
            return 0;
        seenUnits.Add(target);
        var useActionOnDeathComponent = target.GetComponent<ProgramEffectUseActionOnDeath>();
        if (useActionOnDeathComponent == null)
            return 0;
        int score = 0;
        foreach (var pos in useActionOnDeathComponent.action.GetTargets(grid, target.Pos, target))
        {
            var unit = grid.Get(pos);
            if (unit == null || seenUnits.Contains(unit))
                continue;
            if (pos == self.Pos)
            {
                score -= 500;
            }
            else if (unit.UnitTeam == Unit.Team.Player)
            {
                score += 1000 - (unit.HP * 10);
            }
            else if (unit.UnitTeam == Unit.Team.Enemy)
            {
                score -= 10;
            }
            GetDetonateTargetScore(grid, self, unit, ref seenUnits);
        }
        return score;
    }
}
