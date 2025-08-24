using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionFuser : MonoBehaviour
{
    public const string fusedActionDefaultName = "Fused Action";
    [SerializeField] private Action actionTemplatePrefab;

    public Action Fuse(Transform container, IReadOnlyList<Action> actions, bool preview)
    {
        if (actions.Count <= 0)
            return null;
        if (actions.Count == 1)
            return actions[0];
        var fusedAction = actionTemplatePrefab.Validate(container);
        fusedAction.DisplayName = fusedActionDefaultName;
        fusedAction.ActionType = Action.Type.Hybrid;
        string description = "Use ";
        var subActions = new List<SubAction>();
        int sumBaseAP = 0;
        int maxAPCost = 0;
        var highestSlowdownType = Action.Trigger.TurnStart;
        int sumSlowdownInterval = 0;
        int slowdownIntervalCount = 0;
        int maxSlowdown = 0;
        for(int i = 0; i < actions.Count; ++i)
        {
            var action = actions[i];
            if (!preview)
            {
                action.transform.SetParent(fusedAction.transform);
            }
            subActions.AddRange(action.SubActions);
            description += i < actions.Count - 1 ? $"{action.DisplayName}, then " : $"{action.DisplayName}.";
            sumBaseAP += action.BaseAPCost;
            if(action.BaseAPCost > maxAPCost)
            {
                maxAPCost = action.BaseAPCost;
            }
            if(action.SlowdownReset == Action.Trigger.Never)
            {
                if(highestSlowdownType != Action.Trigger.Never)
                {
                    highestSlowdownType = Action.Trigger.Never;
                    slowdownIntervalCount = 1;
                    sumSlowdownInterval = action.SlowdownInterval;
                }
            }
            else if(action.SlowdownReset == Action.Trigger.EncounterStart && highestSlowdownType == Action.Trigger.TurnStart)
            {
                highestSlowdownType = Action.Trigger.EncounterStart;
                slowdownIntervalCount = 1;
                sumSlowdownInterval = action.SlowdownInterval;
            }
            else if(action.SlowdownReset == highestSlowdownType)
            {
                ++slowdownIntervalCount;
                sumSlowdownInterval += action.SlowdownInterval;
            }
            if(action.Slowdown > maxSlowdown)
            {
                maxSlowdown = action.Slowdown;
            }
        }
        fusedAction.SetDescription(description);
        fusedAction.SetSubActions(subActions);
        fusedAction.BaseAPCost = System.Math.Max(0, sumBaseAP - 1);
        fusedAction.SlowdownReset = highestSlowdownType;
        fusedAction.SlowdownInterval = slowdownIntervalCount > 0 ? sumSlowdownInterval / slowdownIntervalCount : 0;
        if (fusedAction.BaseAPCost < maxAPCost)
        {
            fusedAction.SlowdownInterval = Mathf.Min(fusedAction.SlowdownInterval, 2 + fusedAction.BaseAPCost); 
        }
        fusedAction.Slowdown = maxSlowdown;
        return fusedAction;
    }
}
