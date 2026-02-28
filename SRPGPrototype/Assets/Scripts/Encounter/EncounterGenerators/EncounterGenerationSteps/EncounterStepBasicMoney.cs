using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Step", menuName = "Encounter Generation Steps/Money (Basic)")]
public class EncounterStepBasicMoney : EncounterGeneratorStep
{
    [SerializeField] private int baseMoney = 10;
    [SerializeField] private int variance = 0;
    public override void Apply(Metadata metadata, ref Encounter encounter, ref HashSet<Vector2Int> validPositions)
    {
        encounter.giveCompletionMoney = true;
        encounter.baseCompletionMoney += baseMoney;
        encounter.completionMoneyVariance += variance;
    }
}
