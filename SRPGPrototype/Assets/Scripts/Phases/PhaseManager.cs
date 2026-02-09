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
    private int currPhase;

    private System.Action OnActiveEncounterEnd { get; set; }

    public bool EncounterActive { get; set; } = false;
    public bool Transitioning { get; private set; } = true;

    [SerializeField] private List<Phase> phases;

    public BattleGrid Grid => grid;
    [Header("Set In Parent Prefab")]
    [SerializeField] private BattleGrid grid;
    public BattleUI BattleUI => battleUI;
    [SerializeField] private BattleUI battleUI;


    /// <summary>
    /// Initialize the turn count, run the battle start coroutine, and start the first phase
    /// </summary>
    public void StartActiveEncounter(System.Action onEnd, bool start)
    {
        // Initialize Phases
        foreach(var phase in phases)
        {
            phase.Initialize(this);
        }
        // Initialize Turn and Encounter
        Turn = 1;
        EncounterActive = true;
        OnActiveEncounterEnd += onEnd;
        if (start)
        {
            Transitioning = true;
            StartCoroutine(StartEncounterCR());
        }
        else
        {
            Transitioning = false;
        }
    }

    private IEnumerator StartEncounterCR()
    {
        yield return StartCoroutine(ActivePhase.OnPhaseStart());
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
        var waitWhilePaused = new WaitWhile(IsPaused);
        yield return waitWhilePaused;
        yield return StartCoroutine(ActivePhase.OnPhaseEnd());
        yield return waitWhilePaused;
        if (nextPhase >= 0)
        {
            currPhase = nextPhase;
        }
        else if (++currPhase >= phases.Count)
        {
            currPhase = 0;
            ++Turn;
        }
        yield return waitWhilePaused;
        Transitioning = false;
        yield return StartCoroutine(ActivePhase.OnPhaseStart());
        yield return waitWhilePaused;
    }

    private bool IsPaused() => PauseHandle.Paused;
}
