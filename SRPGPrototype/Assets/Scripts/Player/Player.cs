using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerStats stats = new PlayerStats();
    public IEnumerable<Action> Actions => actions;
    private List<Action> actions = new List<Action>();

    public void ClearActions()
    {
        foreach(var action in actions)
        {
            Destroy(action.gameObject);
        }
        actions.Clear();
    }

    public void AddAction(Action action, Program program)
    {
        var newAction = Instantiate(action, transform).GetComponent<Action>();
        newAction.Program = program;
        actions.Add(newAction);
    }
}
