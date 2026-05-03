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
    public System.Action<Unit> OnUnitRemoved { get; set; }
    public Unit.OnAfterAction OnAfterAction { get; set; }
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

    public Queue<System.Func<Coroutine>> Reactions { get; } = new Queue<System.Func<Coroutine>>();

    public static void QueueReaction(System.Func<Coroutine> reaction)
    {

    }

    public static bool HasReactions() => main.Reactions.Count > 0;

    public static void EnqueueReaction(System.Func<Coroutine> reaction)
    {
        main.Reactions.Enqueue(reaction);
    }

    public static bool ProcessReactions(System.Action onComplete)
    {
        if (!HasReactions())
        {
            return false;
        }
        main.StartCoroutine(main.ProcessReactionsCr(onComplete));
        return true;
    }

    private IEnumerator ProcessReactionsCr(System.Action onComplete)
    {
        while(Reactions.Count > 0)
        {
            var reactionCr = Reactions.Dequeue().Invoke();
            if(reactionCr != null)
            {
                yield return reactionCr;
            }
        }
        onComplete?.Invoke();
    }


    private void Awake()
    {
        if(main == null)
        {
            main = this;
        }
    }
}
