using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramAttributeTransient : ProgramAttribute
{
    public int UsesLeft => MaxUses - Uses;
    public int MaxUses { get => maxUses; set => maxUses = value; }
    [SerializeField] private int maxUses = 3;
    public int Uses 
    { 
        get => uses; 
        set
        {
            uses = value;
            if(uses >= maxUses)
            {
                TriggerDestroy();
            }
        }
    }
    private int uses = 0;

    public void TriggerDestroy()
    {
        var shell = program.Shell;
        shell.Uninstall(program, program.Pos, true);
        if (!shell.Compile())
            Debug.LogError("Destroyed program has cause shell compile error");

    }
}
