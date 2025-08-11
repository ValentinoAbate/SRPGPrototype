using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionFuser : MonoBehaviour
{
    [SerializeField] private Action actionTemplatePrefab;

    public Action Fuse(Transform container, IReadOnlyList<Action> actions)
    {
        if (actions.Count <= 0)
            return null;
        if (actions.Count == 1)
            return actions[0];
        var fusedAction = actionTemplatePrefab.Validate(container);
        fusedAction.DisplayName = "MyHybrid";
        fusedAction.ActionType = Action.Type.Hybrid;
        string description = "Use ";
        var subActions = new List<SubAction>();
        int sumBaseAp = 0;
        var highestSlowdownType = Action.Trigger.TurnStart;
        float sumSlowdown = 0;
        int slowdownCount = 0;
        for(int i = 0; i < actions.Count; ++i)
        {
            var action = actions[i];
            action.transform.SetParent(fusedAction.transform);
            subActions.AddRange(action.SubActions);
            sumBaseAp += action.BaseAPCost;
            description += i < actions.Count - 1 ? $"{action.DisplayName}, then " : $"{action.DisplayName}.";
            if(action.SlowdownReset == Action.Trigger.Never)
            {
                if(highestSlowdownType != Action.Trigger.Never)
                {
                    highestSlowdownType = Action.Trigger.Never;
                    slowdownCount = 1;
                    sumSlowdown = action.SlowdownInterval;
                }
            }
            else if(action.SlowdownReset == Action.Trigger.EncounterStart && highestSlowdownType == Action.Trigger.TurnStart)
            {
                highestSlowdownType = Action.Trigger.EncounterStart;
                slowdownCount = 1;
                sumSlowdown = action.SlowdownInterval;
            }
            else if(action.SlowdownReset == highestSlowdownType)
            {
                ++slowdownCount;
                sumSlowdown += action.SlowdownInterval;
            }
        }
        fusedAction.SetDescription(description);
        fusedAction.SetSubActions(subActions);
        fusedAction.BaseAPCost = System.Math.Max(0, sumBaseAp - 1);
        fusedAction.SlowdownReset = highestSlowdownType;
        fusedAction.SlowdownInterval = slowdownCount > 0 ? (int)System.Math.Round(sumSlowdown / slowdownCount, System.MidpointRounding.AwayFromZero) : 0; 
        return fusedAction;
    }
}
