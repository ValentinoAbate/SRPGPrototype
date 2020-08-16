using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandomUtils;
using System.Linq;
using System.Runtime.CompilerServices;

public class EncounterGenerator : MonoBehaviour
{
    public delegate float NextPosWeighting(Vector2Int pos, Encounter encounter, Vector2Int dimensions);
    public delegate float NextUnitWeighting(Unit u, Encounter encounter, Vector2Int dimensions);

    public Encounter Generate(EncounterData data)
    {
        var positions = EnumerateDimensions(data.dimensions);
        var encounter = new Encounter();
        int enemyBudget = data.enemyBudget;
        int lootBudget = data.lootBudget;
        // Initialize encounter values from seed if applicable
        if(data.seed != null)
        {
            encounter.units.AddRange(data.seed.units);
            encounter.reinforcements.AddRange(data.seed.reinforcements);
            positions = positions.Where((pos) => data.seed.units.All((unit) => unit.pos != pos)).ToList();
        }

        // Generate enemies
        encounter.units.AddRange(SpendBudgetAtRandom(data.enemyBudget / 2 + data.obstacleBudget % 2, data.enemies, ref positions));
        SpendBudgetWeighted(data.enemyBudget / 2, data.enemies, ClumpWeight, PassThrough, data.dimensions, encounter, ref positions);

        // Generate obstacles
        encounter.units.AddRange(SpendBudgetAtRandom(data.obstacleBudget / 2 + data.obstacleBudget % 2, data.obstacles, ref positions));
        SpendBudgetWeighted(data.obstacleBudget / 2, data.obstacles, ClumpWeight, PassThrough, data.dimensions, encounter, ref positions);

        // Generate loot placement
        encounter.units.AddRange(SpendBudgetAtRandom(data.lootBudget, data.data, ref positions));

        return encounter;
    }

    private void SpendBudgetWeighted<T>(int budget, List<T> units, NextPosWeighting nextPos, NextUnitWeighting nextUnit, 
        Vector2Int dimensions, Encounter encounter, ref List<Vector2Int> validPositions) where T : Unit, IEncounterUnit
    {
        var availableUnits = new List<T>(units);
        availableUnits.RemoveAll((unit) => unit.EncounterData.cost > budget);
        while (budget > 0 && availableUnits.Count > 0)
        {
            var unit = RandomU.instance.Choice(new WeightedSet<T>(availableUnits, (u) => nextUnit(u, encounter, dimensions)));
            var pos = RandomU.instance.Choice(new WeightedSet<Vector2Int>(validPositions, (p) => nextPos(p, encounter, dimensions)));
            budget -= unit.EncounterData.cost;
            // Remoave all units that are now too expensive
            availableUnits.RemoveAll((u) => u.EncounterData.cost > budget);
            validPositions.Remove(pos);
            encounter.units.Add(new Encounter.UnitEntry() { pos = pos, unit = unit });
        }
    }

    private List<Encounter.UnitEntry> SpendBudgetAtRandom<T>(int budget, List<T> units, ref List<Vector2Int> validPositions) where T : Unit, IEncounterUnit
    {
        var availableUnits = new List<T>(units);
        availableUnits.RemoveAll((unit) => unit.EncounterData.cost > budget);
        var entries = new List<Encounter.UnitEntry>();
        while (budget > 0 && availableUnits.Count > 0)
        {
            var unit = RandomU.instance.Choice(availableUnits);
            var pos = RandomU.instance.Choice(validPositions);
            budget -= unit.EncounterData.cost;
            // Remoave all units that are now too expensive
            availableUnits.RemoveAll((u) => u.EncounterData.cost > budget);
            validPositions.Remove(pos);
            entries.Add(new Encounter.UnitEntry() { pos = pos, unit = unit });
        }
        return entries;
    }

    private float ClumpWeight(Vector2Int pos, Encounter encounter, Vector2Int dimensions)
    {
        float weight = 1;
        foreach(var p in pos.Adjacent())
        {
            if(p.x > 0 && p.y > 0 && p.x < dimensions.x && p.y < dimensions.y 
                && encounter.units.FindIndex((e) => e.pos == p) != -1)
            {
                weight *= 2;
            }
        }
        return weight;
    }

    private float PassThrough(Unit u, Encounter encounter, Vector2Int dimensions) => 1;
    private float PassThrough(Vector2Int pos, Encounter encounter, Vector2Int dimensions) => 1;

    public Encounter.UnitEntry NextUnit(Encounter currEncounter, List<Vector2Int> validPositions, List<EnemyUnit> enemies)
    {
        return new Encounter.UnitEntry() 
        { 
            pos = RandomU.instance.Choice(validPositions), 
            unit = RandomU.instance.Choice(enemies) 
        };
    }

    private List<Vector2Int> EnumerateDimensions(Vector2Int dimensions)
    {
        var ret = new List<Vector2Int>();
        for (int x = 0; x < dimensions.x; ++x)
            for (int y = 0; y < dimensions.y; ++y)
                ret.Add(new Vector2Int(x, y));
        return ret;
    }
}
