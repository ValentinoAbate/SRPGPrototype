using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropComponentProgramStarterAttack : DropComponent<Program>
{
    public override List<Program> GenerateDrop(Loot<Program> loot)
    {
        return new List<Program>() { loot.GetDropCustom(Filter) };
    }

    // Filter out all programs that don't have the correct color and the proper attributes
    static bool Filter(Program p) => p.attributes.HasFlag(Program.Attributes.Starter) && p.color == Program.Color.Red && p.GetComponent<ProgramVariantColor>() == null;
}
