using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CurrentShellViewer : ShellViewer
{
    public TextMeshProUGUI text;
    public Image shellImage;

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
            ClearTriggers();
            text.text = "None";
            shellImage.color = SharedColors.DisabledColor;
            return;
        }
        shellImage.color = Color.white;
        text.text = "Equipped";
        AttachToShell(s);
    }
}
