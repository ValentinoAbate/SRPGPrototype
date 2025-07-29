using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EncounterGenerator;

[CreateAssetMenu(fileName = "Step", menuName = "Encounter Generation Steps/Place Loot (Default)")]
public class EncounterStepPlaceLootDefault : EncounterGeneratorStep
{
    [SerializeField] private LootModifiers lootFlags;
    [SerializeField] private LootUnitSet lootUnits;
    public override void Apply(Metadata metadata, ref Encounter encounter, ref HashSet<Vector2Int> validPositions)
    {
        // Calculate difficulty mod
        float difficultyMod = encounter.Difficulty - metadata.targetDifficulty;

        // Generate Loot category weights
        var difficultyEnum = difficultyMod < 0 ? EncounterDifficulty.Easy : (difficultyMod > 0 ? EncounterDifficulty.Hard : EncounterDifficulty.Normal);
        // Choose weights to use based on difficulty mod
        GetLootWeights(difficultyEnum, out var categoryWeights, out var qualityWeights);
        PlaceLootDefault(lootFlags, categoryWeights, qualityWeights, lootUnits, ref encounter, ref validPositions);
    }
}
