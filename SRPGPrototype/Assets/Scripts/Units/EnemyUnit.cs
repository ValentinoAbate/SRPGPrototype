using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : AIUnit, IEncounterUnit
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
}
