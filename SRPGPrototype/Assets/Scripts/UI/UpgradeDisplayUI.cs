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
        if(p.Upgrades.Count <= 0)
        {
            ShowNone();
            return;
        }
        for (int i = 0; i < upgradeTexts.Length; ++i)
        {
            if (i >= p.Upgrades.Count)
            {
                upgradeTexts[i].text = string.Empty;
                continue;
            }
            upgradeTexts[i].text = p.Upgrades[i].TriggerName + " - " + p.Upgrades[i].Condition.ConditionText;
        }
    }

    public void ShowHidden(Program p)
    {
        if (p.Upgrades.Count <= 0)
        {
            ShowNone();
            return;
        }
        for (int i = 0; i < upgradeTexts.Length; ++i)
        {
            if (i >= p.Upgrades.Count)
            {
                upgradeTexts[i].text = string.Empty;
                continue;
            }
            upgradeTexts[i].text = ProgramDescriptionUI.Hide(p.Upgrades[i].TriggerName) + " - " + ProgramDescriptionUI.Hide(p.Upgrades[i].Condition.ConditionText);
        }
    }
}
