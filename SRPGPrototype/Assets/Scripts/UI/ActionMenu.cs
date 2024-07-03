using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class ActionMenu : MonoBehaviour
{
    [SerializeField] private GameObject actionButtonWeaponPrefab;
    [SerializeField] private GameObject actionButtonMovePrefab;
    [SerializeField] private GameObject actionButtonHybridPrefab;
    [SerializeField] private GameObject actionButtonSkillPrefab;
    [SerializeField] private GameObject actionButtonSpecialPrefab;

    public Transform actionButtonContainer;
    private Dictionary<Action.Type, GameObject> buttonPrefabs;
    private bool showing = false;
    private bool skipInput = false;
    private readonly List<ActionButton> buttons = new List<ActionButton>();

    private void Awake()
    {
        buttonPrefabs = new Dictionary<Action.Type, GameObject>()
        {
            {Action.Type.Weapon, actionButtonWeaponPrefab },
            {Action.Type.Move, actionButtonMovePrefab },
            {Action.Type.Hybrid, actionButtonHybridPrefab },
            {Action.Type.Skill, actionButtonSkillPrefab },
            {Action.Type.Special, actionButtonSpecialPrefab }
        };
    }

    private void Update()
    {
        if (skipInput)
        {
            skipInput = false;
            return;
        }
        if (!showing)
            return;
        CheckForKeyboardInput();
    }

    private void CheckForKeyboardInput()
    {
        for (int i = 0; i < buttons.Count && i < 9; ++i)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                buttons[i].OnHotKey();
                return;
            }
        }
        if (buttons.Count >= 10 && Input.GetKeyDown(KeyCode.Alpha0))
        {
            buttons[9].OnHotKey();
            return;
        }
    }

    public void Show(BattleGrid grid, BattleUI ui, Unit unit, bool fromHotKey)
    {
        // Action Buttons
        var actions = new List<Action>(unit.Actions);
        actions.Sort();
        foreach(var otherUnit in grid)
        {
            var contextActions = otherUnit.GetContextualActions(unit, grid);
            if(contextActions.Count > 0)
            {
                actions.InsertRange(0, contextActions);
            }
        }
        buttons.Clear();
        for (int i = 0; i < actions.Count; i++)
        {
            Action action = actions[i];
            GameObject prefab = buttonPrefabs[action.ActionType];
            var button = Instantiate(prefab, actionButtonContainer).GetComponent<ActionButton>();
            button.Initialize(i, unit, action, ui, Hide);
            buttons.Add(button);
        }
        showing = true;
        skipInput = fromHotKey;
    }

    public void Hide()
    {
        showing = false;
        buttons.Clear();
        actionButtonContainer.DestroyAllChildren();
    }
}
