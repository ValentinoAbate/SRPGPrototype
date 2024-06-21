using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private ShopButtonUI[] buttons;
    [SerializeField] private Canvas canvas;
    [SerializeField] private ProgramDescriptionUI progDescUI;
    [SerializeField] private ShellDescriptionUI shellDescUI;
    [SerializeField] private GameObject buttonContainer2;
    [SerializeField] private int pageSize = 10;


    private System.Action onComplete = null;
    private readonly List<ShopEntry> shopEntries = new List<ShopEntry>();

    private void Awake()
    {
        Hide();
    }

    public void Show(ShopManager.ShopData data, System.Action onComplete)
    {
        this.onComplete = onComplete;
        canvas.enabled = true;
        shopEntries.Clear();
        shopEntries.EnsureCapacity(data.Programs.Count + data.Shells.Count);
        foreach(var program in data.Programs)
        {
            shopEntries.Add(new ProgramShopEntry(data, program, progDescUI));
        }
        foreach(var shell in data.Shells)
        {
            shopEntries.Add(new ShellShopEntry(data, shell, shellDescUI));
        }
        RefreshButtons();
    }

    private void RefreshButtons()
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
        RefreshButtons();
    }

    public void Hide()
    {
        canvas.enabled = false;
        onComplete?.Invoke();
    }
}
