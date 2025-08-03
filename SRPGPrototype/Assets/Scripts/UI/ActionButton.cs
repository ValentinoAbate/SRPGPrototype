using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Button button;
    [SerializeField] private EventTrigger trigger;
    [SerializeField] private Image hotKeyUI;
    [SerializeField] private TextMeshProUGUI hotKeyText;

    private static readonly Color dimColor = new Color(0xC8, 0xC8, 0xC8, 0.5f);

    public bool Interactable
    {
        set
        {
            button.interactable = value;
            hotKeyUI.color = value ? Color.white : dimColor;
        }
    }

    public void Initialize(int index, Unit unit, Action action, BattleUI ui, UnityAction onHide)
    {
        nameText.text = action.DisplayName;
        // Setup hotkey UI
        if(index < 9)
        {
            hotKeyText.text = (index + 1).ToString();
            hotKeyUI.gameObject.SetActive(true);
        }
        else if(index == 9)
        {
            hotKeyText.text = "0";
            hotKeyUI.gameObject.SetActive(true);
        }
        else
        {
            hotKeyUI.gameObject.SetActive(false);
        }
        // Add show action description trigger
        var showActionDesc = new EventTrigger.Entry() { eventID = EventTriggerType.PointerEnter };
        showActionDesc.callback.AddListener((_) => UIManager.main.ActionDescriptionUI.Show(action, unit));
        trigger.triggers.Add(showActionDesc);
        // Add hide action description trigger
        var hideActionDesc = new EventTrigger.Entry() { eventID = EventTriggerType.PointerExit };
        hideActionDesc.callback.AddListener(UIManager.main.HideActionDescriptionUI);
        trigger.triggers.Add(hideActionDesc);
        // Continue if the unit doesn't have enough AP
        if (!unit.CanUseAction(action))
        {
            Interactable = false;
            return;
        }
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(onHide);
        button.onClick.AddListener(() => ui.EnterActionUI(action, unit));
    }

    public void OnHotKey()
    {
        if (!button.interactable)
        {
            return;
        }
        button.onClick?.Invoke();
    }
}
