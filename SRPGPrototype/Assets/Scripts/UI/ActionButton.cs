using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActionButton : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public void Initialize(Action action)
    {
        nameText.text = action.DisplayName;
    }
}
