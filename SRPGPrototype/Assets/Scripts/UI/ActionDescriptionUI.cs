using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;

public class ActionDescriptionUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;
    public TextMeshProUGUI attributesText;
    public TextMeshProUGUI apCostNumberText;
    public TextMeshProUGUI slowdownAmountNumberText;
    public TextMeshProUGUI slowdownUsesNumberText;
    public TextMeshProUGUI slowdownResetText;
    public UpgradeDisplayUI upgradeDisplay;

    public Color textColor;
    public Color modifiedTextColor;

    public void Show(Action action, Unit user = null)
    {
        gameObject.SetActive(true);
        // Name and action type
        nameText.text = action.DisplayName + " - " + action.ActionType.ToString() 
            + " (" + string.Join("/", action.SubActions.Select((s) => s.Subtype.ToString())) + ")";
        descText.text = action.Description;
        // Program attributes
        if(action.Program != null)
        {
            attributesText.text = action.Program.AttributesText;
            upgradeDisplay.Show(action.Program);
        }
        else
        {
            attributesText.text = string.Empty;
            upgradeDisplay.ShowNone();
        }
        if(user == null || user.Speed.IsZero)
        {
            apCostNumberText.text = action.APCost.ToString();
            apCostNumberText.color = textColor;
        }
        else
        {
            int cost = Mathf.Max(0, action.APCost - user.Speed.Value);
            apCostNumberText.text = cost.ToString();
            apCostNumberText.color = modifiedTextColor;
        }
        slowdownAmountNumberText.text = action.Slowdown.ToString();
        slowdownResetText.text = action.SlowdownReset.ToString();
        int timesUsed = 0;
        switch (action.SlowdownReset)
        {
            case Action.Trigger.Never:
                timesUsed = action.TimesUsed - action.FreeUses;
                break;
            case Action.Trigger.TurnStart:
                timesUsed = action.TimesUsedThisTurn - action.FreeUsesThisTurn;
                break;
            case Action.Trigger.EncounterStart:
                timesUsed = action.TimesUsedThisBattle - action.FreeUsesThisBattle;
                break;
        }
        slowdownUsesNumberText.text = (timesUsed % action.SlowdownInterval).ToString() + "/" + action.SlowdownInterval.ToString();
    }

    public void ShowHidden(Action action, Unit user = null)
    {
        const string hiddenChar = "?";
        const string hiddenPattern = @"[^\s]";
        string Hide(string input) => Regex.Replace(input, hiddenPattern, hiddenChar);
        gameObject.SetActive(true);
        // Name and action type
        nameText.text = Hide(action.DisplayName) + " (" + Hide(action.ActionType.ToString()) + ")";
        descText.text = Hide(action.Description); // add description later
        attributesText.text = action.Program != null ? Hide(action.Program.AttributesText) : string.Empty; // this is also for later
        if (user == null || user.Speed.IsZero)
        {
            apCostNumberText.text = Hide(action.APCost.ToString());
            apCostNumberText.color = textColor;
        }
        else
        {
            int cost = Mathf.Max(0, action.APCost - user.Speed.Value);
            apCostNumberText.text = Hide(cost.ToString());
            apCostNumberText.color = modifiedTextColor;
        }
        slowdownAmountNumberText.text = Hide(action.Slowdown.ToString());
        slowdownResetText.text = Hide(action.SlowdownReset.ToString());
        int timesUsed = 0;
        switch (action.SlowdownReset)
        {
            case Action.Trigger.Never:
                timesUsed = action.TimesUsed;
                break;
            case Action.Trigger.TurnStart:
                timesUsed = action.TimesUsedThisTurn;
                break;
            case Action.Trigger.EncounterStart:
                timesUsed = action.TimesUsedThisBattle;
                break;
        }
        slowdownUsesNumberText.text = Hide((timesUsed % action.SlowdownInterval).ToString()) + "/" + Hide(action.SlowdownInterval.ToString());
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
