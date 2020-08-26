using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ProgramDescriptionUI : MonoBehaviour
{
    public TextMeshProUGUI programNameText;
    public TextMeshProUGUI programDescText;
    public TextMeshProUGUI programAttrText;
    public PatternDisplayUI patternDisplay;
    public GameObject patternIconPrefab;

    public void Show(Program p)
    {
        programNameText.text = p.DisplayName;
        programDescText.text = p.Description;
        programAttrText.text = GetAttributesText(p);
        patternDisplay.Show(p.shape, patternIconPrefab, p.ColorValue);
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    private string GetAttributesText(Program p)
    {
        var attTexts = new List<string>();
        if (p.attributes.HasFlag(Program.Attributes.Fixed))
        {
            attTexts.Add("Fixed");
        }
        if (p.attributes.HasFlag(Program.Attributes.Transient))
        {
            var transientAttr = p.GetComponent<ProgramAttributeTransient>();
            string errorText = "Error: No Attribute Component found";
            attTexts.Add("Transient " + (transientAttr == null ? errorText : transientAttr.UsesLeft.ToString()));
        }
        if (attTexts.Count <= 0)
            return string.Empty;
        if (attTexts.Count == 1)
            return attTexts[0];
        return attTexts.Aggregate((s1, s2) => s1 + ", " + s2);
    }
}
