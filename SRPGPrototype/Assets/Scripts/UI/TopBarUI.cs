using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TopBarUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI moneyText;

    public void SetMoneyText(int money)
    {
        moneyText.text = $"Money: ${money}";
    }

    public void SetTitleText(string text)
    {
        titleText.text = text;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
