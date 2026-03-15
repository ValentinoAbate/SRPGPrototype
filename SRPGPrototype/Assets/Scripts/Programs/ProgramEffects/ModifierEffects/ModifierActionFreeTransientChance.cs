using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierActionFreeTransientChance : ModifierAction
{
    public float Chance => chance;
    [Range(0, 1)] [SerializeField] private float chance = 1;
}
