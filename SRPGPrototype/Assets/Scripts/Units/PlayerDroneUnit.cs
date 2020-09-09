using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDroneUnit : PlayerUnit
{
    public override Shell Shell => shell;

    private Shell shell = null;

    public void SetShell(Shell s) => shell = s;
}
