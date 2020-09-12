﻿using System.Collections.Generic;
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
        nameText.text = s.DisplayName;
        descriptionText.text = s.Description;
        // If the shell is an asset (the Install Map is uninitialized), generate compile data from pre-installs
        var compileData = s.InstallMap != null ? s.GetCompileData(out List<Action> newActions) : GenerateCompileData(s, out newActions);
        // Actions text
        actionsText.text = AggregateText(newActions.Select((a) => a.DisplayName).ToList());
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
        // Clean up action objects
        newActions.ForEach((a) => Destroy(a.gameObject));
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public Shell.CompileData GenerateCompileData(Shell s, out List<Action> newActions)
    {
        newActions = new List<Action>();
        var compileData = new Shell.CompileData(new Stats());
        // Look through programs and apply program effects
        foreach (var install in s.preInstalledPrograms)
        {
            foreach (var effect in install.program.Effects)
            {
                compileData.actions.Clear();
                effect.ApplyEffect(install.program, ref compileData);
                // Instantiate new actions
                foreach (var action in compileData.actions)
                {
                    var actionInstance = Instantiate(action.gameObject, transform).GetComponent<Action>();
                    actionInstance.Program = install.program;
                    newActions.Add(actionInstance);
                }
            }
        }
        return compileData;
    }

    private string AggregateText(List<string> text)
    {
        const string separator = ", ";
        const string empty = "None";
        if (text.Count <= 0)
        {
            return empty;
        }
        else if (text.Count == 1)
        {
            return text[0];
        }
        else
        {
            return text.Aggregate((s1, s2) => s1 + separator + s2);
        }
    }
}
