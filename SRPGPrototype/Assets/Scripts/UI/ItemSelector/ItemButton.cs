using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private EventTrigger trigger;
    [SerializeField] private Image colorIcon;
    [SerializeField] private TextMeshProUGUI text;
    public object Item { get; private set; }

    private void SetupAsLoot(string displayName, UnityAction show)
    {
        trigger.triggers.Clear();
        colorIcon.gameObject.SetActive(false);
        text.text = displayName;
        button.SetCallback(show);
        Show();
    }

    public void SetupAsLoot(string displayName, DropComponent<Program> prog, DropComponent<Shell> shell, int money, System.Action onShow, System.Action onComplete, bool midBattle)
    {
        void ShowLoot()
        {
            var loot = PersistantData.main.loot;
            LootData<Program> progLoot = null;
            if (prog != null)
            {
                progLoot = new LootData<Program>(1);
                progLoot.Add(prog.GenerateLootData());
            }
            LootData<Shell> shellLoot = null;
            if (shell != null)
            {
                shellLoot = new LootData<Shell>(1);
                shellLoot.Add(shell.GenerateLootData());
            }
            ICollection<LootUI.MoneyData> moneyLoot = System.Array.Empty<LootUI.MoneyData>();
            if (money != 0)
            {
                moneyLoot = new List<LootUI.MoneyData>() { new LootUI.MoneyData(money) };
            }
            onShow?.Invoke();
            PersistantData.main.loot.ShowUI(PersistantData.main.inventory, progLoot, shellLoot, moneyLoot, onComplete, midBattle);
        }
        SetupAsLoot(displayName, ShowLoot);
    }

    public void SetupAsLoot(string displayName, List<DropComponent<Program>> progs, List<DropComponent<Shell>> shells, List<int> money, System.Action onShow, System.Action onComplete, bool midBattle)
    {
        void ShowLoot()
        {
            var loot = PersistantData.main.loot;
            LootData<Program> progLoot = null;
            if(progs != null && progs.Count > 0)
            {
                progLoot = new LootData<Program>(progs.Count);
                foreach(var prog in progs)
                {
                    progLoot.Add(prog.GenerateLootData());
                }
            }
            LootData<Shell> shellLoot = null;
            if (shells != null && shells.Count > 0)
            {
                shellLoot = new LootData<Shell>(shells.Count);
                foreach (var shell in shells)
                {
                    shellLoot.Add(shell.GenerateLootData());
                }
            }
            ICollection<LootUI.MoneyData> moneyLoot = System.Array.Empty<LootUI.MoneyData>();
            if(money != null && money.Count > 0)
            {
                var moneyList = new List<LootUI.MoneyData>(moneyLoot.Count);
                foreach(var amount in money)
                {
                    moneyList.Add(new LootUI.MoneyData(amount));
                }
                moneyLoot = moneyList;
            }
            onShow?.Invoke();
            PersistantData.main.loot.ShowUI(PersistantData.main.inventory, progLoot, shellLoot, moneyLoot, onComplete, midBattle);
        }
        SetupAsLoot(displayName, ShowLoot);
    }

    public void SetupAsProgram(Program p, Unit selector = null)
    {
        colorIcon.color = p.ColorValue;
        colorIcon.gameObject.SetActive(true);
        text.text = p.DisplayName;
        void ShowProgramDescriptionWindow(BaseEventData _)
        {
            UIManager.main.ProgramDescriptionUI.Show(p, selector);
        }
        trigger.SetHoverCallbacks(ShowProgramDescriptionWindow, UIManager.main.HideProgramDescriptionUI);
        Item = p;
        Show();
    }

    public void SetupAsProgram(Program p, UnityAction callback, Unit selector = null)
    {
        button.SetCallback(callback);
        button.interactable = callback != null;
        SetupAsProgram(p, selector);
    }

    public void SetupAsShell(Shell s, UnityAction callback)
    {
        colorIcon.gameObject.SetActive(false);
        text.text = s.DisplayName;
        void ShowShellDescriptionWindow(BaseEventData _)
        {
            UIManager.main.ShellDescriptionUI.Show(s);
        }
        trigger.SetHoverCallbacks(ShowShellDescriptionWindow, UIManager.main.HideShellDescriptionUI);
        button.SetCallback(callback);
        button.interactable = callback != null;
        Item = s;
        Show();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (!gameObject.activeSelf)
            return;
        gameObject.SetActive(false);
        trigger.OnPointerExit(null);
    }
}
