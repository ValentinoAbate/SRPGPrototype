using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OtherUnitShellViewer : ShellViewer
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image shellImage;

    public Shell Shell { get; private set; }

    public void Cleanup()
    {
        gameObject.SetActive(false);
        Shell = null;
        ClearTriggers();
    }

    public void Initialize(Shell s, int index)
    {
        gameObject.SetActive(true);
        shellImage.color = s.HasSoulCore ? Color.yellow : Color.white;
        text.text = index.ToString();
        Shell = s;
        AttachToShell(s);
    }
}
