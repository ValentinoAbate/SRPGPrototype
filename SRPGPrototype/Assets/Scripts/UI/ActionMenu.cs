using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMenu : MonoBehaviour
{
    public GameObject actionButtonPrefab;
    public GameObject actionButtonMovePrefab;
    public GameObject actionButtonHybridPrefab;

    public Transform actionButtonContainer;
    private Dictionary<Action.Type, GameObject> buttonPrefabs;

    private void Awake()
    {
        buttonPrefabs = new Dictionary<Action.Type, GameObject>()
        {
            {Action.Type.Action, actionButtonPrefab },
            {Action.Type.Move, actionButtonMovePrefab },
            {Action.Type.Hybrid, actionButtonHybridPrefab },
        };
    }

    public void Show(BattleUI ui, Combatant unit)
    {
        var actions = new List<Action>(unit.Actions);
        actions.Sort((a1, a2) => a1.ActionType.CompareTo(a2.ActionType));
        foreach(var action in actions)
        {
            // Continue if the use doesn't have enough AP to use an action
            if (!unit.CanUseAction(action))
                continue;
            GameObject prefab = buttonPrefabs[action.ActionType];
            var button = Instantiate(prefab, actionButtonContainer).GetComponent<ActionButton>();
            button.Initialize(action);
            button.button.onClick.RemoveAllListeners();
            button.button.onClick.AddListener(() => ui.EnterActionUI(action, unit));
            button.button.onClick.AddListener(Hide);
        }
    }

    public void Hide()
    {
        foreach(Transform t in actionButtonContainer)
        {
            Destroy(t.gameObject);
        }
    }
}
