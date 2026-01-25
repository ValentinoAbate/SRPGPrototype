using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Program))]
public abstract class ProgramVariant : MonoBehaviour
{
    public abstract void ApplyVariant(Program p);

    public abstract string Save();

    public abstract void Load(Program p, string savedData);
}
