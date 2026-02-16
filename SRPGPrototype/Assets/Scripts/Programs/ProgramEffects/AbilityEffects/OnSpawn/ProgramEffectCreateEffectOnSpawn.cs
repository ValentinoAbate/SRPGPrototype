using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectCreateEffectOnSpawn : ProgramEffectAddOnSpawnAbility
{
    public override string AbilityName => "Fancy";

    [SerializeField] private GameObject prefab;

    private GameObject instance;

    public override void Ability(BattleGrid grid, Unit unit)
    {
        instance = Instantiate(prefab, unit.FxContainer);
    }

    public override void Cleanup(BattleGrid grid, Unit unit)
    {
        if(instance != null)
        {
            Destroy(instance);
            instance = null;
        }
    }
}
