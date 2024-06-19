using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDroneUnit : PlayerUnit
{
    public override Shell Shell => shell;

    private Shell shell = null;

    public void SetShell(Shell s) => shell = s;

    public override void Kill(BattleGrid grid, Unit killedBy)
    {
        base.Kill(grid, killedBy);
        var inventory = PersistantData.main.inventory;
        foreach(var installedProgram in shell.Programs)
        {
            var program = installedProgram.program;
            if (program.attributes.HasFlag(Program.Attributes.Fixed))
                continue;
            inventory.AddProgram(installedProgram.program);
        }
        inventory.RemoveShell(Shell, true);
        shell = null;
    }
}
