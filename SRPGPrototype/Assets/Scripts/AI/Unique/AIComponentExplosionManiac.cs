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
                var seenUnits = new Dictionary<Unit, int>();
                foreach (var target in detonateTargets)
                {
                    seenUnits.Clear();
                    int score = GetDetonateTargetScore<IDetonatable>(grid, self, target, ref seenUnits, GetAction, target.HP);
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
            if(grid.TryGet(adjPos, out var unit) && unit.UnitTeam == self.UnitTeam)
            {
                score *= 0.25f;
            }
        }
        return score;
    }

    private int GetDetonateTargetScore<T>(BattleGrid grid, AIUnit self, Unit target, ref Dictionary<Unit, int> hitUnits, System.Func<T, Action> getAction, int damage)
    {
        int score = 0;
        if (!hitUnits.ContainsKey(target))
        {
            hitUnits.Add(target, damage);
            score += DetonationHitWeighting(target.Pos, self, target, damage);
        }
        var component = target.GetComponent<T>();
        if (component == null)
            return 0;
        var detonateAction = getAction(component);
        var targetPositions = new List<Vector2Int>(detonateAction.GetTargets(grid, target, target.Pos));
        foreach (var pos in targetPositions)
        {
            if (!grid.TryGet(pos, out var unit) || (hitUnits.TryGetValue(unit, out int dmgTaken) && dmgTaken >= unit.HP))
                continue;
            int simDamage = ActionUtils.SimulateDamageCalc(grid, detonateAction, detonateAction.SubActions[0], target, unit, new ActionEffect.PositionData(target.Pos, target.Pos), 0, targetPositions);
            score += DetonationHitWeighting(pos, self, unit, simDamage);
            if (hitUnits.ContainsKey(unit))
            {
                hitUnits[unit] += simDamage;
            }
            else
            {
                hitUnits[unit] = simDamage;
            }
            if(hitUnits[unit] >= unit.HP)
            {
                score += GetDetonateTargetScore<ProgramEffectUseActionOnDeath>(grid, self, unit, ref hitUnits, GetAction, simDamage);
            }
        }
        return score;
    }

    private static Action GetAction(ProgramEffectUseActionOnDeath p) => p.action;
    private static Action GetAction(IDetonatable d) => d.DetonateAction;

    private int DetonationHitWeighting(Vector2Int pos, Unit self, Unit unit, int damage)
    {
        const int selfHitWeighting = -250;
        const int allyHitWeighting = -10;
        const int targetHitWeighting = 1000;
        const int hpFactor = 10;
        if (pos == self.Pos)
        {
            return selfHitWeighting - damage * hpFactor;
        }
        else if (unit.UnitTeam == Unit.Team.Player)
        {
            return targetHitWeighting - (unit.HP * hpFactor) + damage * hpFactor;
        }
        else if (unit.UnitTeam == Unit.Team.Enemy)
        {
            return allyHitWeighting - damage;
        }
        return 0;
    }
}
