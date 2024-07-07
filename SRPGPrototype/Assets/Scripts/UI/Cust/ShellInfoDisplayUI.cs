using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShellInfoDisplayUI : MonoBehaviour
{
    public TextMeshProUGUI shellHpNumberText;
    public TextMeshProUGUI shellLvNumberText;
    public TextMeshProUGUI shellCapNumberText;
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
        int level = shell.Level;
        var compileData = shell.GetCompileData();
        shellHpNumberText.text = Mathf.Clamp(shell.Stats.HP, 0, compileData.stats.MaxHP).ToString() + "/" + compileData.stats.MaxHP;
        shellLvNumberText.text = level == shell.MaxLevel ? "Max" : level.ToString();
        shellCapNumberText.text = compileData.capacity.ToString() + "/" + shell.CapacityThresholds[level].ToString();
        levelDownButton.interactable = level > 0;
        levelUpButton.interactable = level < shell.MaxLevel;
    }
}
