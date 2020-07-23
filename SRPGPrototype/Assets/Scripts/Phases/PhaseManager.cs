using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// The PhaseManager is the top-level class responsible for managing battle flow. It is essentially a basic state machine.
/// The PhaseManager manages a list of Phase classes which control individual phases of battle.
/// This class implements the public-reference singleton pattern. The singleton is accessible through PhaseManager.main.
/// </summary>
public class PhaseManager : MonoBehaviour, IPausable
{
    public PauseHandle PauseHandle { get => ActivePhase.PauseHandle; set => ActivePhase.PauseHandle = value; }
    public int Turn { get; private set; }
    public Phase ActivePhase { get => phases[currPhase]; }

    public EncounterGenerator encounterGenerator;
    public BattleGrid grid;
    public LootManager loot;

    private List<Phase> phases;
    private int currPhase;
    public bool Transitioning { get; private set; } = true;

    private void Awake()
    {
        InitializePhases();
    }

    /// <summary>
    /// Get the list of phases from the child objects
    /// Logs errors if valid party and enemy phases are not found
    /// </summary>
    private void InitializePhases()
    {
        phases = new List<Phase>();
        phases.AddRange(GetComponentsInChildren<Phase>());
        phases.RemoveAll((p) => !p.enabled);
    }

    /// <summary>
    /// Start is called before the first frame update
    /// Initialize the turn count, run the battle start coroutine, and start the first phase
    /// </summary>
    IEnumerator Start()
    {
        Turn = 1;
        yield return StartCoroutine(StartBattle());
        yield return StartCoroutine(ActivePhase.OnPhaseStart());
        Transitioning = false;
    }

    public void NextPhase()
    {
        if (Transitioning)
            return;
        Transitioning = true;
        StartCoroutine(NextPhaseCr());
    }

    /// <summary>
    /// Do any logic and display any graphics needed to start the battle
    /// Currently placeholder
    /// </summary>
    private IEnumerator StartBattle()
    {
        var encounter = encounterGenerator.Generate(PersistantData.main.combatData.encounterData);
        var units = InitializeUnits(encounter.units);
        phases.ForEach((p) => p.Initialize(units));
        yield break;
    }

    private List<Unit> InitializeUnits(IEnumerable<Encounter.UnitEntry> entries)
    {
        var units = new List<Unit>();
        foreach(var entry in entries)
        {
            var unit = Instantiate(entry.unit).GetComponent<Unit>();
            grid.Add(entry.pos, unit);
            unit.transform.position = grid.GetSpace(unit.Pos);
            units.Add(unit);
        }
        return units;
    }

    public void EndBattle()
    {
        var inv = PersistantData.main.inventory;
        inv.EquippedShell.Stats.DoRepair();
        inv.AddProgram(loot.programLoot.GetDropStandard(Loot<Program>.LootQuality.Standard), true);
        inv.AddProgram(loot.programLoot.GetDropStandard(Loot<Program>.LootQuality.High), true);
        SceneTransitionManager.main.TransitionToScene("Cust");
    }

    /// <summary>
    /// Go to the next phase, waiting for the phases to end and start
    /// If the current phase is the last phase, go to the next turn
    /// </summary>
    private IEnumerator NextPhaseCr(int nextPhase = -1)
    {
        yield return new WaitWhile(() => PauseHandle.Paused);
        yield return StartCoroutine(ActivePhase.OnPhaseEnd());
        yield return new WaitWhile(() => PauseHandle.Paused);
        if(nextPhase >= 0)
        {
            currPhase = nextPhase;
        }
        else if (++currPhase >= phases.Count)
        {
            currPhase = 0;
            ++Turn;
            Debug.Log("It is turn " + Turn);
        }
        yield return new WaitWhile(() => PauseHandle.Paused);
        Debug.Log("Starting Phase: " + ActivePhase.displayName);
        Transitioning = false;
        yield return StartCoroutine(ActivePhase.OnPhaseStart());
        yield return new WaitWhile(() => PauseHandle.Paused);
    }
}
