using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Program))]
public abstract class ProgramVariant : MonoBehaviour
{
    public abstract void ApplyVariant(Program p);

    protected void Initialize()
    {
        ApplyVariant(GetComponent<Program>());
    }

    private void Awake()
    {
        Initialize();
    }
}
