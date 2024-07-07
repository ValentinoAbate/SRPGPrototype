using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterferenceIconUI : MonoBehaviour
{
    [SerializeField] private Image mainImage;
    [SerializeField] private Color jammingColor;
    [SerializeField] private Color lowColorUnderThreshold;
    [SerializeField] private Color lowColorOverThreshold;

    public void UpdateDisplay(Unit.Interference level, bool underThreshold = false)
    {
        if(level == Unit.Interference.None)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);
        if(level == Unit.Interference.Jamming)
        {
            mainImage.color = jammingColor;
            return;
        }
        mainImage.color = underThreshold ? lowColorUnderThreshold : lowColorOverThreshold;
    }
}
