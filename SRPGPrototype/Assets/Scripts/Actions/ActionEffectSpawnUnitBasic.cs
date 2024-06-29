using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectSpawnUnitBasic : ActionEffectSpawnUnit
{
    public Unit unitPrefab;

    protected override GameObject GetUnitPrefab()
    {
        return unitPrefab.gameObject;
    }
}
