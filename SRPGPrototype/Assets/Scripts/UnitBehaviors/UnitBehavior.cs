using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBehavior : MonoBehaviour
{
    [SerializeField] protected AIUnit self;

    protected virtual void Awake()
    {
        AttachListeners();
        self.OnSpawned += AttachListenersOnSpawn;
        self.OnRemoved += CleanupListenersOnRemoved;
        self.OnDeathFn += CleanupListenersOnDeath;
    }

    private void CleanupListenersOnDeath(BattleGrid _, Unit _2, Unit _3)
    {
        CleanupListeners();
    }

    private void CleanupListenersOnRemoved(BattleGrid _, Unit _2)
    {
        CleanupListeners();
    }

    private void AttachListenersOnSpawn(BattleGrid _, Unit _2)
    {
        AttachListeners();
    }

    protected virtual void OnDestroy()
    {
        CleanupListeners();
    }

    protected abstract void AttachListeners();
    protected abstract void CleanupListeners();
}
