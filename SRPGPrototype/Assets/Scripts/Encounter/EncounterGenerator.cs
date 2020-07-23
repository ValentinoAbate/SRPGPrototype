using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandomUtils;
using System.Linq;

public class EncounterGenerator : MonoBehaviour
{
    public delegate float NextPosWeighting(Vector2Int pos, Encounter encounter, Vector2Int dimensions);
    public delegate float NextUnitWeighting(Unit u, Encounter encounter, Vector2Int dimensions);

    public Encounter Generate(EncounterData data) => Generate(data.dimensions, data.budget, data.enemies, data.obstacles, data.seed);

    public Encounter Generate(Vector2Int dimensions, int budget, List<EnemyUnit> enemies, 
        List<ObstacleUnit> obstacles, Encounter seed = null)
    {
        var positions = EnumerateDimensions(dimensions);
        var encounter = new Encounter();
        // Initialize encounter values from seed if applicable
        if(seed != null)
        {
            encounter.units.AddRange(seed.units);
            encounter.reinforcements.AddRange(seed.reinforcements);
            positions = positions.Where((pos) => seed.units.All((unit) => unit.pos != pos)).ToList();
        }
        // Populate with enemies
        var availableUnits = new List<EnemyUnit>(enemies);
        availableUnits.RemoveAll((unit) => unit.EncounterData.cost > budget);
        while (budget > 0 && availableUnits.Count > 0)
        {
            var nextUnit = NextUnit(encounter, positions, availableUnits);
            encounter.units.Add(nextUnit);
            budget -= (nextUnit.unit as EnemyUnit).EncounterData.cost;
            // Remoave all units that are now too expensive
            availableUnits.RemoveAll((unit) => unit.EncounterData.cost > budget);
        }
        return encounter;
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
