using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDroneUnit : PlayerUnit
{
    public override int LinkOutThreshold => 0;
    public override Shell Shell => shell;

    private Shell shell = null;

    public void SetShell(Shell s) => shell = s;

    public override void Kill(BattleGrid grid, Unit killedBy)
    {
        base.Kill(grid, killedBy);
        UIManager.main.RemoveUnitShellFromViewer(Shell);
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

    protected override List<string> SaveArgs()
    {
        var args = base.SaveArgs();
        args.Add(shell.Id.ToString());
        return args;
    }

    protected override void LoadArgs(Queue<string> args, SaveManager.Loader loader)
    {
        base.LoadArgs(args, loader);
        if (args.Count <= 0)
        {
            return;
        }
        var shidStr = args.Dequeue();
        if(int.TryParse(shidStr, out int shid) && loader.LoadedShells.TryGetValue(shid, out var sh))
        {
            SetShell(sh);
        }
    }
}
