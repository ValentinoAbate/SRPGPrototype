using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShellButton : MonoBehaviour
{

    public TextMeshProUGUI shellNameText;
    public Image shellImage;
    public Button equipButton;
    public Button custButton;
    public Color compiledColor = Color.green;
    public Color unCompiledColor = Color.red;

    public void Initialize(Shell s, CustUI uiManager, bool equippedShell = false)
    {
        shellNameText.text = s.DisplayName;
        equipButton.onClick.AddListener(() => uiManager.EquipShell(s));
        custButton.onClick.AddListener(() => uiManager.EnterCust(s));
        shellImage.color = s.Compiled ? compiledColor : unCompiledColor;
        equipButton.gameObject.SetActive(!equippedShell && s.Compiled);
    }
}
