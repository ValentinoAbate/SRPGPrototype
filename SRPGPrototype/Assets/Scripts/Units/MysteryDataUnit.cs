using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryDataUnit : AIUnit, IEncounterUnit
{
    #region IEncounterUnit implementation

    public EncounterUnitData EncounterData => encounterData;
    [SerializeField] private EncounterUnitData encounterData = null;

    #endregion

    public override AIComponent<Unit> AI => ai;

    private AIComponent<Unit> ai;

    private void Awake()
    {
        ai = GetComponent<AIComponent<Unit>>();
    }

    public override IEnumerator OnBattleEnd(EncounterManager manager)
    {
        if(Dead)
            yield break;
        var progDrops = GetComponentsInChildren<DropComponent<Program>>();
        foreach (var drop in progDrops)
            manager.GenerateProgramLoot += drop.GenerateDrop;
        var shellDrops = GetComponentsInChildren<DropComponent<Shell>>();
        foreach (var drop in shellDrops)
            manager.GenerateShellLoot += drop.GenerateDrop;
    }
}
