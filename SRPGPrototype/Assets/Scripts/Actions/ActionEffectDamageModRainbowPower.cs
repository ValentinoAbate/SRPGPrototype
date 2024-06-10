using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActionEffectDamageModRainbowPower : ActionEffectDamage
{
    [SerializeField] private Modifier mod;
    public override int BaseDamage(BattleGrid grid, Action action, Unit user, IReadOnlyList<Vector2Int> targetPositions)
    {
        return BasicDamage(action, user);
    }

    public override int BasicDamage(Action action, Unit user)
    {
        if (mod.Program == null)
            return 0;
        var shell = mod.Program.Shell;
        if(shell == null)
        {
            shell = PersistantData.main.inventory.EquippedShell;
        }
        var colors = new Dictionary<Program.Color, int>(5)
        {
            { Program.Color.Blue, 0 },
            { Program.Color.Red, 0 },
            { Program.Color.Yellow, 0 },
            { Program.Color.Green, 0 },
            { Program.Color.White, 0 },
        };
        foreach (var prog in shell.Programs)
        {
            var color = prog.program.color;
            if (!colors.ContainsKey(color))
            {
                colors.Add(color, 1);
                continue;
            }
            colors[color]++;
        }
        if (colors.Count <= 0)
            return 0;
        int lowestInstalled = int.MaxValue;
        foreach(var kvp in colors)
        {
            if(kvp.Value < lowestInstalled)
            {
                lowestInstalled = kvp.Value;
            }
        }
        return lowestInstalled;
    }

    public override int TargetModifier(BattleGrid grid, Action action, Unit user, Unit target, PositionData targetData)
    {
        return 0;
    }
}
