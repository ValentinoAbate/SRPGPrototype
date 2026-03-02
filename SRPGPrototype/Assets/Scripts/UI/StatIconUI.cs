using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatIconUI : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI numberText;

    public void SetValue(int value)
    {
        if(value == 0)
        {
            gameObject.SetActive(false);
            return;
        }
        numberText.text = value.ToString();
        gameObject.SetActive(true);
    }
}
