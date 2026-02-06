using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProgramEffect : MonoBehaviour
{
    public virtual void Initialize(Program program) { }
    public abstract void ApplyEffect(ref Shell.CompileData data);

    public virtual bool CanSave(bool isBattle) => false;
    public virtual string Save(bool isBattle) => string.Empty;
    public virtual void Load(string data, bool isBattle, Unit unit) { }
}
