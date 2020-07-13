using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour, IEnumerable<SubAction>
{
    public enum Type
    { 
        Move,
        Action,
        Hybrid,
    }


    public Type ActionType => type;
    [SerializeField] Type type = Type.Action;

    public int APCost => apCost;
    [SerializeField] private int apCost = 1;

    public int MaxUsesPerBattle => maxUsesPerBattle;
    [SerializeField] private int maxUsesPerBattle = 10;

    public int TimesUsedThisBattle { get; set; } = 0;

    public int MaxUsesPerTurn => maxUsesPerTurn;
    [SerializeField] private int maxUsesPerTurn = 3;

    public int TimesUsedThisTurn { get; set; } = 0;

    public string DisplayName => displayName;
    [SerializeField] private string displayName = string.Empty;

    public List<SubAction> subActions;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        var singleAction = GetComponent<SubAction>();
        subActions = singleAction != null ? new List<SubAction> { singleAction } :
            new List<SubAction>(GetComponentsInChildren<SubAction>());
    }

    public IEnumerator<SubAction> GetEnumerator()
    {
        return subActions.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return subActions.GetEnumerator();
    }
}
