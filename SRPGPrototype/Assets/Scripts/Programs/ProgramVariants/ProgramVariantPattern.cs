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

    private int index = -1;

    public override void ApplyVariant(Program p)
    {
        if (patterns.Length <= 0 || (float)RandomU.instance.RandomDouble() > variantChance)
        {
            index = -1;
            return;
        }
        p.shape = useWeights ? RandomU.instance.Choice(patterns, weights, out index) : RandomU.instance.Choice(patterns, out index);
    }

    public override void Load(Program p, string savedData)
    {
        if (int.TryParse(savedData, out index) && index >= 0 && index < patterns.Length)
        {
            p.shape = patterns[index];
        }
    }

    public override string Save()
    {
        return index.ToString();
    }
}
