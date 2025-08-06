using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterUnitMimicData : MonoBehaviour
{
    [SerializeField] private List<Unit> units;
    [SerializeField] private List<float> unitWeights;
    public Unit GetUnit()
    {
        return RandomU.instance.ChoiceWeightsOptional(units, unitWeights);
    }
}
