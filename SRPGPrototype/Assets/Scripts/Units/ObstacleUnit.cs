using UnityEngine;

public class ObstacleUnit : AIUnit, IEncounterUnit
{
    #region IEncounterUnit implementation

    public EncounterUnitData EncounterData => encounterData;
    [SerializeField] private EncounterUnitData encounterData = null;

    #endregion
}
