using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramAttributeTransient : ProgramAttribute
{
    public int UsesLeft => MaxUses - Uses;
    public int MaxUses { get => maxUses; set => maxUses = value; }
    [SerializeField] private int maxUses = 3;
    public int Uses { get; set; } = 0;

    public void Use(BattleGrid grid, Unit user)
    {
        if(++Uses >= MaxUses)
        {
            program.Shell.DestroyProgram(program, grid, user);
        }
    }
    public void SetUsesLeft(int usesLeft)
    {
        Uses = MaxUses - usesLeft;
    }
}
