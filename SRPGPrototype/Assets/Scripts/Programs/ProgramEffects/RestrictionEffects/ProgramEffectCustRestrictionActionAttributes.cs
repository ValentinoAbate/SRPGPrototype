using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ProgramEffectCustRestrictionActionAttributes : ProgramEffectCustRestriction
{
    [SerializeField] private Modifier.FilterMode attributeFilterMode = Modifier.FilterMode.NeedOne;
    [SerializeField] private Program.Attributes attributes = Program.Attributes.None;

    protected override string RestrictionName => "Action Programs must have specific attribute(s)";

    public bool AppliesTo(Program p)
    {
        if (attributeFilterMode == Modifier.FilterMode.NeedOne)
        {
            if ((attributes & p.attributes) == 0)
                return false;
        }
        else if (attributeFilterMode == Modifier.FilterMode.NeedAll)
        {
            if (!p.attributes.HasFlag(attributes))
                return false;
        }
        return true;
    }

    protected override bool Restriction(Shell shell, out string errorMessage)
    {
        foreach(var install in shell.Programs)
        {
            var program = install.program;
            if (program.Effects.Any((e) => e is ProgramEffectAddAction) && !AppliesTo(program))
            {
                errorMessage = "Compille Error: Program " + program.DisplayName + " gives actions and does not have the proper attribute(s)";
                return true;
            }
        }
        errorMessage = noErrorMessage;
        return false;
    }
}
