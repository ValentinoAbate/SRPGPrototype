using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActionDescriptionUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;
    public TextMeshProUGUI attributesText;
    public TextMeshProUGUI apCostNumberText;
    public TextMeshProUGUI slowdownAmountNumberText;
    public TextMeshProUGUI slowdownUsesNumberText;
    public TextMeshProUGUI slowdownResetText;

    public Color textColor;
    public Color modifiedTextColor;

    public void Show(Action action, Unit user)
    {
        gameObject.SetActive(true);
        // Name and action type
        nameText.text = action.DisplayName + " (" + action.ActionType.ToString() + ")";
        descText.text = string.Empty; // add description later
        attributesText.text = string.Empty; // this is also for later
        if(user.Speed.IsZero)
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
                timesUsed = action.TimesUsed;
                break;
            case Action.Trigger.TurnStart:
                timesUsed = action.TimesUsedThisTurn;
                break;
            case Action.Trigger.EncounterStart:
                timesUsed = action.TimesUsedThisBattle;
                break;
        }
        slowdownUsesNumberText.text = (timesUsed % action.SlowdownInterval).ToString() + "/" + action.SlowdownInterval.ToString();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
