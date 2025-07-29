using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ProgramNumber : DynamicNumber
{
    public enum Type
    { 
        Constant,
        NumberOfInstallsAtrribute,
        NumberOfInstallsRarity,
        NumberOfInstallsColor,
        ShellRows = 5,
        ShellCols,
        InstallX,
        InstallY,
        NumberOfEmptySpaces,
    }

    [SerializeField]
    private int constant = 1;
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
        var shell = p.Shell;
        if(shell == null)
        {
            shell = PersistantData.main.inventory.EquippedShell;
            if (shell == null)
                return number;
        }
        if(type == Type.NumberOfInstallsAtrribute)
        {
            number = shell.Programs.Count((p2) => p2.program.attributes.HasFlag(attributes));
        }
        else if(type == Type.NumberOfInstallsColor)
        {
            number = shell.Programs.Count((p2) => p2.program.color == color);
        }
        else if(type == Type.NumberOfInstallsRarity)
        {
            number = shell.Programs.Count((p2) => p2.program.Rarity == rarity);
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
            number = shell.CustArea.Dimensions.x;
        }
        else if (type == Type.ShellRows)
        {
            number = shell.CustArea.Dimensions.y;
        }
        else if(type == Type.NumberOfEmptySpaces)
        {
            number = shell.CustArea.Offsets.Count((pos) => shell.InstallMap[pos.x, pos.y] == null);
        }
        return Value(number);
    }
}
