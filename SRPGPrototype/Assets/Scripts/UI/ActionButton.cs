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

    public bool Enabled
    {
        set
        {
            float a = value ? 1 : 0.5f;
            button.colors = new ColorBlock() {
                normalColor = button.colors.normalColor.WithAlpha(a),
                highlightedColor = button.colors.highlightedColor.WithAlpha(a),
                pressedColor = button.colors.pressedColor.WithAlpha(a),
                disabledColor = button.colors.disabledColor.WithAlpha(a),
                selectedColor = button.colors.selectedColor.WithAlpha(a),
                fadeDuration = button.colors.fadeDuration,
                colorMultiplier = button.colors.colorMultiplier,
            };
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
        showActionDesc.callback.AddListener((_) => UIManager.main.ActionDescriptionUI.Show(action, unit, ui.grid));
        trigger.triggers.Add(showActionDesc);
        // Add hide action description trigger
        var hideActionDesc = new EventTrigger.Entry() { eventID = EventTriggerType.PointerExit };
        hideActionDesc.callback.AddListener(UIManager.main.HideActionDescriptionUI);
        trigger.triggers.Add(hideActionDesc);
        button.onClick.RemoveAllListeners();
        // Continue if the unit doesn't have enough AP
        if (!action.CanUse(ui.grid, unit, out string failMessage))
        {
            button.onClick.AddListener(() => UIManager.main.PlayFloatText(transform.position, failMessage, Color.white, true));
            Enabled = false;
        }
        else
        {
            button.onClick.AddListener(onHide);
            button.onClick.AddListener(() => ui.EnterActionUI(action, unit));
            Enabled = true;
        }
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
