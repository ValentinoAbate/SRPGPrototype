using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierActionNoSlowdownChance : ModifierAction
{
    public float Chance => chance;
    [Range(0, 1)] [SerializeField] private float chance = 1;
    public override bool AppliesTo(SubAction sub)
    {
        return true;
    }
}
