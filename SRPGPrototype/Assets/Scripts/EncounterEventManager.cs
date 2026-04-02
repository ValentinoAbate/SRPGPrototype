using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterEventManager : MonoBehaviour
{
    public static EncounterEventManager main;
    public static bool Ready => main != null;
    public Unit.OnDeath OnUnitDeath { get; set; }
    public Unit.OnDamaged OnUnitDamaged { get; set; }
    public System.Action<Unit> OnUnitSpawned { get; set; }
    public System.Action<Unit> OnUnitRemoved{ get; set; }
    public DelayedEffectQueue DelayedEffectQueue => delayedEffectQueue;
    [SerializeField] private DelayedEffectQueue delayedEffectQueue;

    public static void StartQueueDelay()
    {
        if (!Ready)
            return;
        main.DelayedEffectQueue.StartQueueDelay();
    }

    public static void EndQueueDelay()
    {
        if (!Ready)
            return;
        main.DelayedEffectQueue.EndQueueDelay();
    }

    public static void EnqueueDelayedEffect(System.Action action)
    {
        if (!Ready)
            return;
        main.DelayedEffectQueue.Enqueue(action);
    }

    private void Awake()
    {
        if(main == null)
        {
            main = this;
        }
    }
}
