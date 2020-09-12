using System.Collections.Generic;

public class ProgramEffectCustRestrictionNumberOfColors : ProgramEffectCustRestriction
{
    public int number = 4;

    protected override string RestrictionName => "No More Than " + number.ToString() + " Different Colors";

    protected override bool Restriction(Shell shell, out string errorMessage)
    {
        var colors = new HashSet<Program.Color>();
        foreach(var install in shell.Programs)
        {
            if (!colors.Contains(install.program.color))
                colors.Add(install.program.color);
        }
        if(colors.Count > number)
        {
            errorMessage = "Compille Error: More than " + number.ToString() + " different colors.";
            return true;
        }
        errorMessage = noErrorMessage;
        return false;
    }
}
