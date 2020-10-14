using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleUnit : AIUnit, IEncounterUnit
{
    #region IEncounterUnit implementation

    public EncounterUnitData EncounterData => encounterData;
    [SerializeField] private EncounterUnitData encounterData = null;

    #endregion

    public override AIComponent<AIUnit> AI => ai;

    private AIComponent<AIUnit> ai;

    private void Awake()
    {
        ai = GetComponent<AIComponent<AIUnit>>();
    }
}
