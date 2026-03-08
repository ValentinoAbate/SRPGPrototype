using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Phase : MonoBehaviour, IPausable
{
    public const bool playTransitionGrahics = true;
    public abstract PauseHandle PauseHandle { get; set; }

    protected List<T> GetUnits<T>()
    {
        var units = new List<T>();
        foreach (var unit in Manager.Grid)
        {
            if (unit != null && !unit.Dead && UnitPredicate(unit) && unit is T tUnit)
                units.Add(tUnit);
        }
        return units;
    }

    public string DisplayName => displayName;
    [SerializeField] private string displayName;
    [SerializeField] private GameObject phaseTransitionPrefab;

    private PhaseManager Manager { get; set; }
    protected BattleUI BattleUI => Manager.BattleUI;
    protected BattleGrid Grid => Manager.Grid;

    public void Initialize(PhaseManager manager)
    {
        Manager = manager;
    }

    public abstract IEnumerator OnPhaseStart(bool isBattleStart);
    public abstract IEnumerator OnPhaseEnd();

    protected abstract bool UnitPredicate(Unit unit);

    protected void EndPhase()
    {
        Manager.NextPhase();
    }

    protected void EndBattle()
    {
        Manager.EndActiveEncounter();
    }
    protected IEnumerator PlayTransition()
    {
        if (playTransitionGrahics && phaseTransitionPrefab != null)
        {
            var cutIn = Instantiate(phaseTransitionPrefab);
            yield return new WaitForSeconds(1.5f);
            Destroy(cutIn);
        }
    }
}
