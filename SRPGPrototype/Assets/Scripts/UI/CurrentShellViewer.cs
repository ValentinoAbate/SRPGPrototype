using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CurrentShellViewer : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Image shellImage;
    public EventTrigger[] triggers;
    [SerializeField] private ShellDescriptionUI shellDescriptionUI;

    public void Start()
    {
        PersistantData.main.inventory.OnEquippedShellChanged += Initialize;
        Initialize(PersistantData.main.inventory.EquippedShell);
    }
    public void OnDestroy()
    {
        PersistantData.main.inventory.OnEquippedShellChanged -= Initialize;
    }

    public void Initialize(Shell s)
    {
        if (PersistantData.main.inventory.EquippedShell == null)
        {
            foreach (var trigger in triggers)
            {
                trigger.triggers.Clear();
            }
            text.text = "No Equipped Shell";
            shellImage.color = SharedColors.DisabledColor;
            return;
        }
        shellImage.color = Color.white;
        text.text = "View Equipped Shell";
        foreach (var trigger in triggers)
        {
            trigger.triggers.Clear();
            var hover = new EventTrigger.Entry() { eventID = EventTriggerType.PointerEnter };
            hover.callback.AddListener((data) => shellDescriptionUI.Show(s));
            var hoverExit = new EventTrigger.Entry() { eventID = EventTriggerType.PointerExit };
            hoverExit.callback.AddListener((data) => shellDescriptionUI.Hide());
            trigger.triggers.Add(hover);
            trigger.triggers.Add(hoverExit);
        }
    }
}
