using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionEffectMoveDirectionBasic : ActionEffectMoveDirection
{
    [SerializeField] private int numSpaces = 1;
    [SerializeField] private bool scale = false;
    protected override int GetNumSpaces(Unit target, PositionData targetData)
    {
        if (scale)
        {
            return numSpaces + (target.Pos.ChebyshevDistance(targetData.selectedPos) - 1);
        }
        return numSpaces;
    }
}
