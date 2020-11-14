using System.Linq;
using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;

public class ProgramDescriptionUI : MonoBehaviour
{
    private const string noUpgradeText = "None";
    public TextMeshProUGUI programNameText;
    public TextMeshProUGUI programDescText;
    public TextMeshProUGUI programAttrText;
    public PatternDisplayUI patternDisplay;
    public GameObject patternIconPrefab;
    public ActionDescriptionUI actionDisplay;
    public TextMeshProUGUI[] upgradeTexts = new TextMeshProUGUI[4];

    public void Show(Program p)
    {
        programNameText.text = p.DisplayName;
        programAttrText.text = p.AttributesText;
        patternDisplay.Show(p.shape, patternIconPrefab, p.ColorValue);
        gameObject.SetActive(true);
        var actions = p.Effects.Where((e) => e is ProgramEffectAddAction).Select((e) => e as ProgramEffectAddAction).ToList();
        if (actions.Count == 1)
        {
            actionDisplay.gameObject.SetActive(true);
            actionDisplay.Show(actions[0].action);
            programNameText.text += (" - " + p.Description);
            programDescText.text = string.Empty;
        }
        else
        {
            actionDisplay.gameObject.SetActive(false);
            programDescText.text = p.Description;
        }

        for (int i = 0; i < upgradeTexts.Length; ++i)
        {
            if (i >= p.Triggers.Length)
            {
                upgradeTexts[i].text = i == 0 ? noUpgradeText : string.Empty;
                continue;
            }
            upgradeTexts[i].text = p.Triggers[i].TriggerName + " - " + p.Triggers[i].Condition.ConditionText;
            if (p.Triggers[i] is ProgramHatch)
            {
                upgradeTexts[i].text += "(Hatch)";
            }
        }
    }

    /// <summary>
    /// Show the description with the info hidden. Used in the upgrade previews
    /// </summary>
    public void ShowHidden(Program p)
    {
        const string hiddenChar = "?";
        const string hiddenPattern = @"[^\s]";
        string Hide(string input) => Regex.Replace(input, hiddenPattern, hiddenChar);
        programNameText.text = Hide(p.DisplayName);
        programAttrText.text = Hide(p.AttributesText);
        patternDisplay.Show(p.shape, patternIconPrefab, p.ColorValue);
        gameObject.SetActive(true);
        var actions = p.Effects.Where((e) => e is ProgramEffectAddAction).Select((e) => e as ProgramEffectAddAction).ToList();
        if (actions.Count == 1)
        {
            actionDisplay.gameObject.SetActive(true);
            actionDisplay.ShowHidden(actions[0].action);
            programNameText.text += (" - " + Hide(p.Description));
            programDescText.text = string.Empty;
        }
        else
        {
            actionDisplay.gameObject.SetActive(false);
            programDescText.text = Hide(p.Description);
        }
        for (int i = 0; i < upgradeTexts.Length; ++i)
        {
            if (i >= p.Triggers.Length)
            {
                upgradeTexts[i].text = i == 0 ? noUpgradeText : string.Empty;
                continue;
            }
            upgradeTexts[i].text = Hide(p.Triggers[i].TriggerName) + " - " + Hide(p.Triggers[i].Condition.ConditionText);
            if (p.Triggers[i] is ProgramHatch)
            {
                upgradeTexts[i].text += "(Hatch)";
            }
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
