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
    [SerializeField] private Transform hotKeyUI;
    [SerializeField] private TextMeshProUGUI hotKeyText;

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
        showActionDesc.callback.AddListener((data) => ui.actionDescription.Show(action, unit));
        trigger.triggers.Add(showActionDesc);
        // Add hide action description trigger
        var hideActionDesc = new EventTrigger.Entry() { eventID = EventTriggerType.PointerExit };
        hideActionDesc.callback.AddListener((data) => ui.actionDescription.Hide());
        trigger.triggers.Add(hideActionDesc);
        // Continue if the unit doesn't have enough AP
        if (!unit.CanUseAction(action))
        {
            button.interactable = false;
            return;
        }
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => ui.EnterActionUI(action, unit));
        button.onClick.AddListener(onHide);
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
