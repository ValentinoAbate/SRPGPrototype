using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionEffectMoveDirectionBasic : ActionEffectMoveDirection
{
    [SerializeField] private int numSpaces = 1;
    protected override int GetNumSpaces()
    {
        return numSpaces;
    }
}
