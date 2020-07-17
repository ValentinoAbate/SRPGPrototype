using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
            {Action.Type.Standard, actionButtonPrefab },
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
            GameObject prefab = buttonPrefabs[action.ActionType];
            var button = Instantiate(prefab, actionButtonContainer).GetComponent<ActionButton>();
            button.Initialize(action);
            // Add show action description trigger
            var showActionDesc = new EventTrigger.Entry() { eventID = EventTriggerType.PointerEnter };
            showActionDesc.callback.AddListener((data) => ui.actionDescription.Show(action));
            button.trigger.triggers.Add(showActionDesc);
            // Add hide action description trigger
            var hideActionDesc = new EventTrigger.Entry() { eventID = EventTriggerType.PointerExit };
            hideActionDesc.callback.AddListener((data) => ui.actionDescription.Hide());
            button.trigger.triggers.Add(hideActionDesc);
            // Continue if the use doesn't have enough AP or uses
            if (!action.Usable || !unit.CanUseAction(action))
            {
                button.button.interactable = false;
                continue;
            }
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
