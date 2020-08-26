using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ProgramNumber
{
    public enum Type
    { 
        Constant,
        NumberOfInstallsAtrribute,
        NumberOfInstallsRarity,
        NumberOfInstallsColor,
        EmptySpaces,
        ShellRows,
        ShellCols,
        InstallX,
        InstallY,
    }

    [SerializeField]
    private int min = int.MinValue;
    [SerializeField]
    private int max = int.MaxValue;
    [SerializeField]
    private int constant = 1;
    [SerializeField]
    private int baseAmount = 0;
    [SerializeField]
    private int modifier = 0;
    [SerializeField]
    private int multiplier = 1;
    [SerializeField]
    private Type type = Type.Constant;
    [SerializeField]
    private Program.Attributes attributes = Program.Attributes.None;
    [SerializeField]
    private Program.Color color = Program.Color.White;
    [SerializeField]
    private Rarity rarity = Rarity.Common;


    public int Value(Program p)
    {
        if (type == Type.Constant)
            return constant;
        int number = 0;
        if(type == Type.NumberOfInstallsAtrribute)
        {
            number = p.Shell.Programs.Count((p2) => p2.program.attributes.HasFlag(attributes));
        }
        else if(type == Type.NumberOfInstallsColor)
        {
            number = p.Shell.Programs.Count((p2) => p2.program.color == color);
        }
        else if(type == Type.NumberOfInstallsRarity)
        {
            number = p.Shell.Programs.Count((p2) => p2.program.Rarity == rarity);
        }
        else if(type == Type.InstallX)
        {
            number = p.Pos.x;
        }
        else if(type == Type.InstallY)
        {
            number = p.Pos.y;
        }
        else if(type == Type.ShellCols)
        {
            number = p.Shell.CustArea.Dimensions.x;
        }
        else if (type == Type.ShellRows)
        {
            number = p.Shell.CustArea.Dimensions.y;
        }
        int modNumber = baseAmount + ((number + modifier) * multiplier);
        return Mathf.Clamp(modNumber, min, max);
    }
}
