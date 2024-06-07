﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectModifyCapacity : ProgramEffect
{
    public int amount = 1;
    public override void ApplyEffect(ref Shell.CompileData data)
    {
        data.capacity += amount;
    }
}
