using RandomUtils;
using UnityEngine;

public class ProgramVariantColor : ProgramVariant
{
    [SerializeField] [Range(0, 1)] private float variantChance = 1;
    [SerializeField] private Program.Color[] colors = new Program.Color[3];
    [SerializeField] private bool useWeights = false;
    [SerializeField] private float[] weights = new float[0];

    public override void ApplyVariant(Program p)
    {
        if (colors.Length <= 0 || (float)RandomU.instance.RandomDouble() > variantChance)
            return;
        p.color = useWeights ? RandomU.instance.Choice(colors, weights) : RandomU.instance.Choice(colors);
    }
}
