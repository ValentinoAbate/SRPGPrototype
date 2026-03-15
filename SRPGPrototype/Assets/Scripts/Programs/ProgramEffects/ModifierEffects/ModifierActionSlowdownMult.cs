using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierActionSlowdownMult : ModifierAction
{
    public float Multiplier => multiplier;
    [SerializeField] private float multiplier;
    public override bool AppliesTo(Action a)
    {
        return a.SlowdownReset != Action.Trigger.Never && base.AppliesTo(a);
    }
}
