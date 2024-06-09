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
    public IReadOnlyList<Modifier> Modifiers => modifiers;
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
        var modifiedProgs = new List<Program>();
        switch (scope)
        {
            case Scope.Adjacent:
                modifiedProgs.AddRange(AdjacentPrograms(shell, program));
                break;
        }
        var applicableModifiers = new List<Modifier>(modifiers.Length);
        foreach (var prog in modifiedProgs)
        {
            if (prog == program)
                continue;
            applicableModifiers.Clear();
            foreach(var modifier in modifiers)
            {
                if (modifier.AppliesTo(prog))
                {
                    applicableModifiers.Add(modifier);
                }
            }
            if (applicableModifiers.Count <= 0)
                continue;
            if (!modifierMap.ContainsKey(prog))
                modifierMap.Add(prog, new List<Modifier>());
            modifierMap[prog].AddRange(applicableModifiers);
        }
    }

    public static IEnumerable<Program> AdjacentPrograms(Shell shell, Program program)
    {
        if (!shell.InstallPositions.ContainsKey(program))
            return System.Array.Empty<Program>();
        var adjacentPrograms = new HashSet<Program>();
        var adjArray = new Vector2Int[4];
        foreach(var position in shell.InstallPositions[program])
        {
            position.Adjacent(ref adjArray);
            foreach(var adjPos in adjArray)
            {
                var adjProgram = shell.GetProgram(adjPos);
                if (adjProgram == null || adjProgram == program || adjacentPrograms.Contains(adjProgram))
                    continue;
                adjacentPrograms.Add(adjProgram);
            }
        }
        return adjacentPrograms;
    }
}
