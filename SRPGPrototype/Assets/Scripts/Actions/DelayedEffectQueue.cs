using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedEffectQueue : MonoBehaviour
{
    public bool IsDelayed { get; private set; }

    private readonly Queue<System.Action> delayedActions = new Queue<System.Action>();

    public void StartQueueDelay()
    {
        IsDelayed = true;
    }

    public void EndQueueDelay()
    {
        IsDelayed = false;
        if (delayedActions.Count <= 0)
            return;
        var actionList = new List<System.Action>(delayedActions.Count);
        actionList.AddRange(delayedActions);
        delayedActions.Clear();
        foreach(var action in actionList)
        {
            action?.Invoke();
        }
    }

    public void Enqueue(System.Action action)
    {
        if (!IsDelayed)
        {
            action?.Invoke();
            return;
        }
        delayedActions.Enqueue(action);
    }
}
