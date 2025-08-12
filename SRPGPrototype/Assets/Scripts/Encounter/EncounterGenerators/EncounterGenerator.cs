using RandomUtils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extensions.VectorIntDimensionUtils;

public abstract class EncounterGenerator : ScriptableObject
{
    public enum Type
    {
        Encounter,
        Shop,
        Boss,
        Rest,
    }

    [System.Flags]
    public enum LootModifiers
    {
        None = 0,
        Shell = 1,
        BossCapacity = 2,
        Bonus = 4,
        NoNormalLoot = 8,
        BonusMoney = 16,
        BonusRandom = 32,
    }
    public enum EncounterDifficulty
    {
        Normal,
        Easy,
        Hard
    }

    [Header("General")]
    [SerializeField] private Type encounterType;
    [SerializeField] private Vector2Int dimensions = new Vector2Int(8, 8);

    public abstract Encounter Generate(string mapSymbol, int encounterNumber);

    protected void InitializeEncounter(string mapSymbol, int encounterNumber, out HashSet<Vector2Int> positions, out Encounter encounter)
    {
        positions = new HashSet<Vector2Int>(dimensions.Enumerate());
        encounter = new Encounter() { nameOverride = $"{encounterType} {mapSymbol}-{encounterNumber}", dimensions = dimensions };
    }

    protected void ApplySeed(Encounter seed, ref Encounter encounter, ref HashSet<Vector2Int> positions)
    {
        // Initialize encounter values from seed if applicable
        if (seed != null)
        {
            var outOfBoundsUnits = new List<Encounter.UnitEntry>(seed.Units.Count);
            // Place seed units
            foreach (var entry in seed.Units)
            {
                // Position is invalid, generate a proper position later
                if (!positions.Contains(entry.pos))
                {
                    outOfBoundsUnits.Add(entry);
                    continue;
                }
                encounter.AddUnit(entry);
                positions.Remove(entry.pos);
            }
            // Process seed spawn positions
            foreach(var spawnPosition in seed.spawnPositions)
            {
                positions.Remove(spawnPosition);
                encounter.spawnPositions.Add(spawnPosition);
            }
            // Generate positions for any unit entries that had negative positions
            foreach (var entry in outOfBoundsUnits)
            {
                if (positions.Count <= 0)
                    break;
                Vector2Int pos = RandomU.instance.Choice(positions);
                encounter.AddUnit(entry.unit, pos);
                positions.Remove(pos);
            }
            if (!string.IsNullOrEmpty(seed.nameOverride))
            {
                encounter.nameOverride = seed.nameOverride;
            }
        }
    }
}
