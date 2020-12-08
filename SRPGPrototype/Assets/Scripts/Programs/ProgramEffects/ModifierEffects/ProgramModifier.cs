using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Extensions.VectorIntDimensionUtils;

public class ProgramModifier : MonoBehaviour
{
    public enum Scope
    { 
        Adjacent,
    }

    [SerializeField] private Scope scope = Scope.Adjacent;

    [SerializeField] private GameObject modifierContainer = null;
    private Modifier[] modifiers;

    private void Awake()
    {
        if(modifierContainer != null)
        {
            modifiers = modifierContainer.GetComponentsInChildren<Modifier>(true);
        }
        else
        {
            modifiers = GetComponentsInChildren<Modifier>(true);
        }

    }

    public void LinkModifiers(Program program, ref Dictionary<Program, List<Modifier>> modifierMap)
    {
        var shell = program.Shell;
        List<Program> modifiedProgs = new List<Program>();
        switch (scope)
        {
            case Scope.Adjacent:
                modifiedProgs.AddRange(AdjacentPrograms(shell, program));
                break;
        }
        foreach (var prog in modifiedProgs)
        {
            if (prog == program)
                continue;
            var applicableModifiers = modifiers.Where((m) => m.AppliesTo(prog));
            if (applicableModifiers.Count() <= 0)
                continue;
            if (!modifierMap.ContainsKey(prog))
                modifierMap.Add(prog, new List<Modifier>());
            modifierMap[prog].AddRange(applicableModifiers);
        }
    }

    private IEnumerable<Program> AdjacentPrograms(Shell shell, Program program)
    {
        return shell.InstallPositions[program].SelectMany((p) => p.Adjacent())
                                              .Where((p) => p.IsBoundedBy(shell.CustArea.Dimensions) && shell.InstallMap[p.x, p.y] != null)
                                              .Select((p) => shell.InstallMap[p.x, p.y])
                                              .Distinct();
    }
}
