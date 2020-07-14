using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectAddAction : ProgramEffect
{
    public Action action;

    public override void ApplyEffect(Program program, ref PlayerStats stats, ref List<Player.ProgramAction> actions)
    {
        actions.Add(new Player.ProgramAction(program, action));
    }
}
