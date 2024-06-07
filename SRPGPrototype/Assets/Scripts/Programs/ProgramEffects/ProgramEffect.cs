using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProgramEffect : MonoBehaviour
{
    public virtual void Initialize(Program program) { }
    public abstract void ApplyEffect(ref Shell.CompileData data);
}
