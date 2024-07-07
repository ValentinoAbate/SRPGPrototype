using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShellInfoDisplayUI : MonoBehaviour
{
    public TextMeshProUGUI shellNameText;
    public TextMeshProUGUI shellHpNumberText;
    public TextMeshProUGUI shellApNumberText;
    public TextMeshProUGUI shellRepairNumberText;
    public TextMeshProUGUI shellCapacityText;
    public TextMeshProUGUI shellCapacityNumberText;
    public Button levelUpButton;
    public Button levelDownButton;

    public void Initialize(UnityAction onLevelUp, UnityAction onLevelDown)
    {
        levelUpButton.onClick.RemoveAllListeners();
        levelUpButton.onClick.AddListener(onLevelUp);
        levelDownButton.onClick.RemoveAllListeners();
        levelDownButton.onClick.AddListener(onLevelDown);
    }

    public void UpdateUI(Shell shell)
    {
        var compileData = shell.GetCompileData();
        shellNameText.text = shell.DisplayName;
        shellHpNumberText.text = $"{Mathf.Clamp(shell.Stats.HP, 0, compileData.stats.MaxHP)}/{compileData.stats.MaxHP}";
        shellApNumberText.text = compileData.stats.MaxAP.ToString();
        shellRepairNumberText.text = compileData.stats.Repair.ToString();
        string levelString = shell.Level == shell.MaxLevel ? "Max" : $"Lv{shell.DisplayLevel}";
        shellCapacityText.text = $"Capacity ({levelString})";
        shellCapacityNumberText.text = $"{compileData.capacity}/{shell.CapacityThresholds[shell.Level]}";
        levelDownButton.interactable = shell.Level > 0;
        levelUpButton.interactable = shell.Level < shell.MaxLevel;
    }
}
