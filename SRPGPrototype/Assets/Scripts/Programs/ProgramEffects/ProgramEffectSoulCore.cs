using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectSoulCore : ProgramEffect
{
    public GameObject unitPrefab;
    public override void ApplyEffect(Program program, ref Shell.CompileData data)
    {
        data.soulCoreUnitPrefab = unitPrefab;
    }
}
