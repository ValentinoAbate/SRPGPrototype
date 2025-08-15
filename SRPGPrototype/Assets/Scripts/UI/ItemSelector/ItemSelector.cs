using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemSelector : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private ItemButton itemButtonPrefab;

    private readonly List<ItemButton> buttons = new List<ItemButton>();

    public void Show(string header, IReadOnlyList<object> items, Unit selector, ISelectableItemHandler handler)
    {
        headerText.text = header;
        for (int i = 0; i < items.Count; ++i)
        {
            if(i >= buttons.Count)
            {
                buttons.Add(Instantiate(itemButtonPrefab, buttonContainer));
            }
            var item = items[i];
            var button = buttons[i];
            void OnClick()
            {
                if (handler.TrySelectItem(item))
                {
                    button.Hide();
                }
            }
            if(item is Program program)
            {
                button.SetupAsProgram(program, OnClick, selector);
            }
            else if(item is Shell shell)
            {
                button.SetupAsShell(shell, OnClick);
            }
        }
        for (int i = items.Count; i < buttons.Count; i++)
        {
            buttons[i].Hide();
        }
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ReturnItem(object item)
    {
        foreach(var button in buttons)
        {
            if(button.Item == item)
            {
                button.Show();
            }
        }
    }
}
