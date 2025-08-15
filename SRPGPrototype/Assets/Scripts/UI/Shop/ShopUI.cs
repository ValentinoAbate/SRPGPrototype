using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private ShopButtonUI[] buttons;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject buttonContainer2;
    [SerializeField] private int pageSize = 10;

    private System.Action onComplete = null;
    private readonly List<ShopEntry> shopEntries = new List<ShopEntry>();

    private void Awake()
    {
        Hide();
    }

    public void Show(ShopManager.ShopData data, Unit shopper, System.Action onComplete)
    {
        UIManager.main.BattleUI?.SetUIEnabled(false);
        UIManager.main.TopBarUI.SetTitleText(data.DisplayName, true);
        this.onComplete = onComplete;
        canvas.enabled = true;
        shopEntries.Clear();
        shopEntries.EnsureCapacity(data.Programs.Count + data.Shells.Count);
        foreach(var program in data.Programs)
        {
            shopEntries.Add(new ProgramShopEntry(data, program, shopper));
        }
        foreach(var shell in data.Shells)
        {
            shopEntries.Add(new ShellShopEntry(data, shell));
        }
        Refresh();
    }

    private void Refresh()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            var button = buttons[i];
            if (i < shopEntries.Count)
            {
                button.Show(shopEntries[i], this, i);
            }
            else
            {
                button.Hide();
            }
        }
        buttonContainer2.SetActive(shopEntries.Count > pageSize);
    }

    public void OnPurchaseComplete(int index)
    {
        shopEntries.RemoveAt(index);
        Refresh();
    }

    public void Hide()
    {
        canvas.enabled = false;
        UIManager.main.TopBarUI.EndTempTitleText();
        UIManager.main.BattleUI?.SetUIEnabled(true);
        onComplete?.Invoke();
    }
}
