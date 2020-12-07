using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Extensions.VectorIntDimensionUtils;

public class ProgramEffectModifier : ProgramEffect
{
    public enum Scope
    { 
        Adjacent,
    }

    [SerializeField] private Scope scope = Scope.Adjacent;

    private ProgramModifier[] modifiers;

    private void Awake()
    {
        modifiers = GetComponents<ProgramModifier>();
    }

    public override void ApplyEffect(Program program, ref Shell.CompileData data)
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
            if (!data.modifierMap.ContainsKey(prog))
                data.modifierMap.Add(prog, new List<ProgramModifier>());
            data.modifierMap[prog].AddRange(applicableModifiers);
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
