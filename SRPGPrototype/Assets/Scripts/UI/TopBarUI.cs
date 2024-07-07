using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TopBarUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI moneyText;
    private readonly Stack<string> titleStack = new Stack<string>();

    public void SetMoneyText(int money)
    {
        moneyText.text = $"Money: ${money}";
    }

    public void SetTitleText(string text, bool temp = false)
    {
        if (temp)
        {
            titleStack.Push(titleText.text);
        }
        titleText.text = text;
    }

    public void EndTempTitleText()
    {
        if (titleStack.Count <= 0)
            return;
        titleText.text = titleStack.Pop();
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
