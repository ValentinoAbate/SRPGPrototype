using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Program))]
public abstract class ProgramAttribute : MonoBehaviour
{
    protected Program program;

    private void Awake()
    {
        Initialize();
    }

    protected void Initialize()
    {
        program = GetComponent<Program>();
    }
}
