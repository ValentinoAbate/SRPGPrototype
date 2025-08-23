using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitDescriptionUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;
    public TextMeshProUGUI interferenceText;
    public TextMeshProUGUI hpNumberText;
    public TextMeshProUGUI apNumberText;
    public TextMeshProUGUI repairNumberText;
    public TextMeshProUGUI powerNumberText;
    public TextMeshProUGUI speedNumberText;
    public TextMeshProUGUI breakNumberText;

    public void Show(Unit unit)
    {
        gameObject.SetActive(true);
        // Name and description
        nameText.text = unit.DisplayName + " (" + unit.UnitTeam.ToString() + ")";
        descText.text = TextMacros.ApplyUnitTextMacros(unit.Description, unit);
        // Interference
        interferenceText.text = unit.InterferenceLevel.ToString();
        // Stats
        hpNumberText.text = unit.HP.ToString() + "/" + unit.MaxHP.ToString();
        apNumberText.text = unit.AP.ToString() + "/" + unit.MaxAP.ToString();
        repairNumberText.text = unit.Repair.ToString();
        powerNumberText.text = unit.Power.Value.ToString();
        speedNumberText.text = unit.Speed.Value.ToString();
        breakNumberText.text = unit.Break.Value.ToString();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
