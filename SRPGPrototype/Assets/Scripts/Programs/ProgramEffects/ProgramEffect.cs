using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProgramEffect : MonoBehaviour
{
    public abstract void ApplyEffect(Program program, ref Shell.CompileData data);
}
