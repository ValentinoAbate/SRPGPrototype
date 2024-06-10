using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ShellDescriptionUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI actionsText;
    public TextMeshProUGUI abilitiesText;
    public TextMeshProUGUI restrictionsText;
    public TextMeshProUGUI levelNumberText;
    public TextMeshProUGUI capacityNumberText;
    public TextMeshProUGUI hpNumberText;
    public TextMeshProUGUI apNumberText;
    public TextMeshProUGUI repairNumberText;
    public TextMeshProUGUI compiledStatusText;
    public TextMeshProUGUI bugNumberText;
    public ShellPatternDisplayUI shellPatternDisplay;

    public void Show(Shell s)
    {
        if(s.Compiled && s.HasSoulCore)
        {
            nameText.text = $"{s.DisplayName} (Soul Core)";
        }
        else
        {
            nameText.text = s.DisplayName;
        }
        descriptionText.text = s.Description;
        // If the shell is an asset, compile data will generate from pre-installs
        var compileData = s.GetCompileData();
        // Actions text
        actionsText.text = AggregateText(compileData.actions.Select((a) => a.DisplayName));
        // Abilities Text
        abilitiesText.text = AggregateText(compileData.abilityNames);
        // Restricions text
        restrictionsText.text = AggregateText(compileData.restrictionNames);
        // Stats
        levelNumberText.text = s.Level.ToString();
        capacityNumberText.text = compileData.capacity.ToString() + "/" + s.CapacityThresholds[s.Level];
        hpNumberText.text = Mathf.Clamp(s.Stats.HP, 0, compileData.stats.MaxHP).ToString() + "/" + compileData.stats.MaxHP;
        apNumberText.text = compileData.stats.MaxAP.ToString();
        repairNumberText.text = compileData.stats.Repair.ToString();
        compiledStatusText.text = s.Compiled ? "Yes" : "No";
        bugNumberText.text = "N/A";
        // Show shell graphics
        shellPatternDisplay.Show(s);
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private string AggregateText(IEnumerable<string> text)
    {
        const string separator = ", ";
        const string empty = "None";
        var modText = HandleDuplicateText(text).ToArray();
        if (modText.Length <= 0)
        {
            return empty;
        }
        else if (modText.Length == 1)
        {
            return modText[0];
        }
        else
        {
            return modText.Aggregate((s1, s2) => s1 + separator + s2);
        }
    }

    private IEnumerable<string> HandleDuplicateText(IEnumerable<string> text)
    {
        var counts = new Dictionary<string, int>();
        foreach(var s in text)
        {
            if (counts.ContainsKey(s))
                ++counts[s];
            else
                counts.Add(s, 1);
        }
        return text.Distinct().Select((s) => counts[s] > 1 ? s + " (x" + counts[s].ToString() + ")" : s);
    }
}
