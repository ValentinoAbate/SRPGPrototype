using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class ActionMenu : MonoBehaviour
{
    public GameObject actionButtonWeaponPrefab;
    public GameObject actionButtonMovePrefab;
    public GameObject actionButtonHybridPrefab;
    public GameObject actionButtonSkillPrefab;
    public GameObject linkoutButtonPrefab;

    public Transform actionButtonContainer;
    private Dictionary<Action.Type, GameObject> buttonPrefabs;

    private void Awake()
    {
        buttonPrefabs = new Dictionary<Action.Type, GameObject>()
        {
            {Action.Type.Weapon, actionButtonWeaponPrefab },
            {Action.Type.Move, actionButtonMovePrefab },
            {Action.Type.Hybrid, actionButtonHybridPrefab },
            {Action.Type.Skill, actionButtonSkillPrefab },
        };
    }

    public void Show(BattleGrid grid, BattleUI ui, Unit unit, System.Action OnLinkout)
    {
        // Action Buttons
        var actions = new List<Action>(unit.Actions);
        actions.Sort((a1, a2) => a1.ActionType.CompareTo(a2.ActionType));
        foreach(var action in actions)
        {
            GameObject prefab = buttonPrefabs[action.ActionType];
            var button = Instantiate(prefab, actionButtonContainer).GetComponent<ActionButton>();
            button.Initialize(action);
            // Add show action description trigger
            var showActionDesc = new EventTrigger.Entry() { eventID = EventTriggerType.PointerEnter };
            showActionDesc.callback.AddListener((data) => ui.actionDescription.Show(action, unit));
            button.trigger.triggers.Add(showActionDesc);
            // Add hide action description trigger
            var hideActionDesc = new EventTrigger.Entry() { eventID = EventTriggerType.PointerExit };
            hideActionDesc.callback.AddListener((data) => ui.actionDescription.Hide());
            button.trigger.triggers.Add(hideActionDesc);
            // Continue if the unit doesn't have enough AP
            if (!unit.CanUseAction(action))
            {
                button.button.interactable = false;
                continue;
            }
            button.button.onClick.RemoveAllListeners();
            button.button.onClick.AddListener(() => ui.EnterActionUI(action, unit));
            button.button.onClick.AddListener(Hide);
        }
        // Link Out Button
        var linkoutButton = Instantiate(linkoutButtonPrefab, actionButtonContainer).GetComponent<Button>();
        var units = grid.FindAll();
        if(units.Any((u) => u.InterferenceLevel == Unit.Interference.Jamming)
            || units.Count((u) => u.InterferenceLevel == Unit.Interference.Low) > 2)
        {
            linkoutButton.interactable = false;
        }
        else
        {
            linkoutButton.interactable = true;
            linkoutButton.onClick.AddListener(() => OnLinkout());
        }

    }

    public void Hide()
    {
        actionButtonContainer.DestroyAllChildren();
    }
}
