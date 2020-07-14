using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerStats stats = new PlayerStats();
    public IEnumerable<ProgramAction> Actions => actions;
    private List<ProgramAction> actions = new List<ProgramAction>();

    public void ClearActions()
    {
        foreach(var pA in actions)
        {
            Destroy(pA.action.gameObject);
        }
        actions.Clear();
    }

    public void AddAction(ProgramAction action)
    {
        var newAction = Instantiate(action.action, transform).GetComponent<Action>();
        actions.Add(new ProgramAction(action.program, newAction));
    }

    public void AddActions(IEnumerable<ProgramAction> actions)
    {
        foreach(var pA in actions)
            AddAction(pA);
    }

    public struct ProgramAction
    {
        public Program program;
        public Action action;
        public ProgramAction(Program program, Action action)
        {
            this.program = program;
            this.action = action;
        }
    }

}
