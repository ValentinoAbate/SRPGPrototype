using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandomUtils;

public class ProgramVariantPattern : ProgramVariant
{
    [SerializeField] [Range(0, 1)] private float variantChance = 1;
    [SerializeField] private Pattern[] patterns = new Pattern[1];
    [SerializeField] private bool useWeights = false;
    [SerializeField] private float[] weights = new float[0];
    public override void ApplyVariant(Program p)
    {
        if (patterns.Length <= 0 || (float)RandomU.instance.RandomDouble() > variantChance)
            return;
        p.shape = useWeights ? RandomU.instance.Choice(patterns, weights) : RandomU.instance.Choice(patterns);
    }
}
