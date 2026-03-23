using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShellDealerValidator", menuName = "Validators/Unit/Shell Dealer")]
public class ShellDealerValidator : UnitValidator
{
    [SerializeField] private int shellThreshold = 2;
    public override bool IsValid(Unit item)
    {
        // Player must have less shells than the threshold
        if (PersistantData.main.inventory.Shells.Count >= shellThreshold)
            return false;
        // Player must have a free soul core
        if (!HasSoulCore(PersistantData.main.inventory.Programs))
            return false;
        // Player must not have an empty shell
        foreach(var shell in PersistantData.main.inventory.Shells)
        {
            if (shell == PersistantData.main.inventory.EquippedShell)
                continue;
            if (!HasSoulCore(shell))
                return false;
        }
        return true;
    }

    private static bool HasSoulCore(IEnumerable<Program> programs)
    {
        foreach (var program in programs)
        {
            if (ProgramFilters.HasAttributes(program, Program.Attributes.SoulCore))
            {
                return true;
            }
        }
        return false;
    }

    private static bool HasSoulCore(Shell shell)
    {
        if (shell.Compiled)
        {
            return shell.HasSoulCore;
        }
        foreach (var install in shell.Programs)
        {
            if (ProgramFilters.HasAttributes(install.program, Program.Attributes.SoulCore))
            {
                return true;
            }
        }
        return false;
    }
}
