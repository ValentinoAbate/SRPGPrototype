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
    public TextMeshProUGUI usesPerTurnNumberText;
    public TextMeshProUGUI usesPerBattleNumberText;

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
        }
        else
        {
            int cost = Mathf.Max(0, action.APCost - user.Speed.Value);
            apCostNumberText.text = cost.ToString() + " (Modified by Speed: " + user.Speed.Value + " )";
        }

        //int usesThisTurn = action.MaxUsesPerTurn - action.TimesUsedThisTurn;
        //usesPerTurnNumberText.text = usesThisTurn + " / " + action.MaxUsesPerTurn;
        //int usesThisBattle = action.MaxUsesPerBattle - action.TimesUsedThisBattle;
        //usesPerBattleNumberText.text = usesThisBattle + " / " + action.MaxUsesPerBattle;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
