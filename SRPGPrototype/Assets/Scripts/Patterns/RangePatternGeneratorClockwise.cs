using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangePatternGeneratorClockwise : RangePatternGenerator, IActionMacroTextProvider
{
    private static readonly string[] clockwiseText = new string[]
    {
        "above you",
        "up/right of you",
        "right of you",
        "down/right of you",
        "below you",
        "down/left of you",
        "left of you",
        "up/left of you",
    };

    private int index = 0;
    public override IEnumerable<Vector2Int> Generate(BattleGrid grid, Vector2Int userPos, Unit user)
    {
        yield return userPos + Vector2IntUtils.Clockwise(index);
    }

    public override void OnUsed()
    {
        index = (index + 1) % Vector2IntUtils.ClockwiseLength;
    }

    public override int MaxDistance(BattleGrid grid)
    {
        return 2;
    }

    public string GetText(Queue<int> indices, Unit unit, BattleGrid grid)
    {
        if (index < 0)
        {
            return "N/A";
        }
        return clockwiseText[index % clockwiseText.Length];
    }

    public override bool CanSave(bool isBattle) => true;

    public override string Save(bool isBattle)
    {
        return index.ToString();
    }

    public override void Load(string data, bool isBattle)
    {
        if(int.TryParse(data, out int savedIndex))
        {
            index = savedIndex;
        }
    }
}
