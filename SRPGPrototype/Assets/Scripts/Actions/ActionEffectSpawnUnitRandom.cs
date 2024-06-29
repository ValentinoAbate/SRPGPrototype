using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectSpawnUnitRandom : ActionEffectSpawnUnit
{
    [SerializeField] private GameObject[] unitPrefabs;
    [SerializeField] private float[] weights;

    protected override GameObject GetUnitPrefab()
    {
        return RandomUtils.RandomU.instance.Choice(unitPrefabs, weights);
    }
}
