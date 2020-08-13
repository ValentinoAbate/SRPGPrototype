using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandomUtils;
using System.Linq;

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
        // Generate loot placement
        encounter.units.AddRange(SpendBudgetAtRandom(data.lootBudget, data.data, ref positions));
        // Generate enemies
        encounter.units.AddRange(SpendBudgetAtRandom(data.enemyBudget, data.enemies, ref positions));

        return encounter;
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
