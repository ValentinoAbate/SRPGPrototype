﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Phase : MonoBehaviour, IPausable
{
    public const bool playTransitionGrahics = true;
    public abstract PauseHandle PauseHandle { get; set; }

    public string displayName;
    public PhaseManager manager;
    public GameObject phaseTransitionPrefab;

    public virtual void Initialize(IEnumerable<Unit> allUnits)
    {

    }

    public abstract IEnumerator OnPhaseStart();
    public abstract IEnumerator OnPhaseEnd();

    protected void EndPhase()
    {
        manager.NextPhase();
    }

    protected void EndBattle()
    {
        manager.EndBattle();
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
