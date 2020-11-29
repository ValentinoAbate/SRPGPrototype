using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeDisplayUI : MonoBehaviour
{
    private const string noUpgradeText = "None";
    [SerializeField]
    private TextMeshProUGUI[] upgradeTexts = new TextMeshProUGUI[4];
    
    public void ShowNone()
    { 
        foreach(var text in upgradeTexts)
        {
            text.text = string.Empty;
        }
        upgradeTexts[0].text = noUpgradeText;
    }

    public void Show(Program p)
    {
        if(p.Triggers.Length <= 0)
        {
            ShowNone();
            return;
        }
        for (int i = 0; i < upgradeTexts.Length; ++i)
        {
            if (i >= p.Triggers.Length)
            {
                upgradeTexts[i].text = string.Empty;
                continue;
            }
            upgradeTexts[i].text = p.Triggers[i].TriggerName + " - " + p.Triggers[i].Condition.ConditionText;
            if (p.Triggers[i] is ProgramHatch)
            {
                upgradeTexts[i].text += "(Hatch)";
            }
        }
    }

    public void ShowHidden(Program p)
    {
        if (p.Triggers.Length <= 0)
        {
            ShowNone();
            return;
        }
        for (int i = 0; i < upgradeTexts.Length; ++i)
        {
            if (i >= p.Triggers.Length)
            {
                upgradeTexts[i].text = string.Empty;
                continue;
            }
            upgradeTexts[i].text = ProgramDescriptionUI.Hide(p.Triggers[i].TriggerName) + " - " + ProgramDescriptionUI.Hide(p.Triggers[i].Condition.ConditionText);
            if (p.Triggers[i] is ProgramHatch)
            {
                upgradeTexts[i].text += "(Hatch)";
            }
        }
    }
}
