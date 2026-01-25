using RandomUtils;
using UnityEngine;

public class ProgramVariantColor : ProgramVariant
{
    [SerializeField] [Range(0, 1)] private float variantChance = 1;
    [SerializeField] private Program.Color[] colors = new Program.Color[3];
    [SerializeField] private bool useWeights = false;
    [SerializeField] private float[] weights = new float[0];

    private int index = -1;

    public override void ApplyVariant(Program p)
    {
        if (colors.Length <= 0 || (float)RandomU.instance.RandomDouble() > variantChance)
        {
            index = -1;
            return;
        }
        p.color = useWeights ? RandomU.instance.Choice(colors, weights, out index) : RandomU.instance.Choice(colors, out index);
    }

    public override void Load(Program p, string savedData)
    {
        if(int.TryParse(savedData, out index) && index >= 0 && index < colors.Length)
        {
            p.color = colors[index];
        }
    }

    public override string Save()
    {
        return index.ToString();
    }
}
