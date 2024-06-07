using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramHatch : ProgramTrigger
{
    [SerializeField] private Shell[] shells;
    [SerializeField] private Program[] programs;

    public override void Initialize(Program program)
    {
        throw new System.NotImplementedException();
    }
}
