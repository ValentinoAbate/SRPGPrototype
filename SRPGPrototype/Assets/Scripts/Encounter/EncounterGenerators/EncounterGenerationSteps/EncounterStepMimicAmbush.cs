using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Step", menuName = "Encounter Generation Steps/Mimic Ambush")]
public class EncounterStepMimicAmbush : EncounterGeneratorStep
{
    [SerializeField] private List<int> num = new List<int>();
    [SerializeField] private List<float> numWeights = new List<float>();

    public override void Apply(Metadata metadata, ref Encounter encounter, ref HashSet<Vector2Int> validPositions)
    {
        var alreadyTaken = new HashSet<Vector2Int>();
        foreach(var entry in encounter.ambushUnits)
        {
            alreadyTaken.Add(entry.pos);
        }
        var mimicTargets = new Dictionary<Vector2Int, EncounterUnitMimicData>(encounter.Units.Count - alreadyTaken.Count);
        foreach(var entry in encounter.Units)
        {
            var mimicComponent = entry.unit.GetComponent<EncounterUnitMimicData>();
            if (mimicComponent == null)
                continue;
            mimicTargets.Add(entry.pos, mimicComponent);
        }
        if (mimicTargets.Count <= 0)
            return;
        int numMimics = RandomU.instance.ChoiceWeightsOptional(num, numWeights);
        for(int i = 0; i < numMimics; ++i)
        {
            var mimic = RandomU.instance.Choice(mimicTargets);
            encounter.ambushUnits.Add(new Encounter.UnitEntry(mimic.Value.GetUnit(), mimic.Key));
            mimicTargets.Remove(mimic.Key);
            if (mimicTargets.Count <= 0)
                break;
        }
    }
}
