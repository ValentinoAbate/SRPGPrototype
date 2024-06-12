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

    private System.Action OnActiveEncounterEnd { get; set; }

    private List<Phase> phases;
    public IReadOnlyList<Unit> Units => units;
    private List<Unit> units;
    private int currPhase;

    public bool EncounterActive { get; set; } = false;
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

    public void AddUnit(Unit u)
    {
        units.Add(u);
    }

    /// <summary>
    /// Initialize the turn count, run the battle start coroutine, and start the first phase
    /// </summary>
    public IEnumerator StartActiveEncounter(List<Unit> encounterUnits, System.Action onEnd)
    {
        Turn = 1;
        units = encounterUnits;
        EncounterActive = true;
        phases.ForEach((p) => p.InitializePhase(units));
        OnActiveEncounterEnd += onEnd;
        yield return StartCoroutine(ShowEncounterStartUI());
        yield return StartCoroutine(ActivePhase.OnPhaseStart(units));
        Transitioning = false;
    }

    /// <summary>
    /// Go to the next phase. Doesn't work if we are already transitioning to the next phase or if the encounter has ended
    /// </summary>
    public void NextPhase()
    {
        if (Transitioning || !EncounterActive)
            return;
        Transitioning = true;
        StartCoroutine(NextPhaseCr());
    }

    /// <summary>
    /// Do any logic and display any graphics needed to start the battle
    /// Currently placeholder
    /// </summary>
    private IEnumerator ShowEncounterStartUI()
    {
        yield break;
    }

    public void EndActiveEncounter()
    {
        EncounterActive = false;
        OnActiveEncounterEnd.Invoke();
        OnActiveEncounterEnd = null;
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
        yield return StartCoroutine(ActivePhase.OnPhaseStart(units));
        yield return new WaitWhile(() => PauseHandle.Paused);
    }
}
