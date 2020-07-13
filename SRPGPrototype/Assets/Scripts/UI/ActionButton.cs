using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Button button;
    public void Initialize(Action action)
    {
        nameText.text = action.DisplayName;
    }
}
