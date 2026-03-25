using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramTraderUI : MonoBehaviour, ISelectableItemHandler
{
    [SerializeField] private List<ProgramItemButton> tradeArguments;
    [SerializeField] private Transform tradeButtonContainer;
    [SerializeField] private ItemButton tradeButtonPrefab;
    [SerializeField] private ColorDropData[] colorDrops;
    [SerializeField] private DropComponent<Program> commonDrop;
    [SerializeField] private DropComponent<Program> uncommonDrop;
    [SerializeField] private DropComponent<Program> rareDrop;
    [SerializeField] private DropComponent<Program> rarePlusDrop;
    [SerializeField] private DropComponent<Program> gambleDrop;

    private Dictionary<Program.Color, DropComponent<Program>> colorDict;
    private readonly List<ItemButton> tradeButtons = new List<ItemButton>();
    private PrefabPool<ItemButton> tradeButtonPool;
    private int argumentIndex = 0;
    private Unit user;

    private void Awake()
    {
        colorDict = new Dictionary<Program.Color, DropComponent<Program>>(colorDrops.Length);
        foreach(var drop in colorDrops)
        {
            colorDict[drop.color] = drop.dropComponent;
        }
        tradeButtonPool = new PrefabPool<ItemButton>(tradeButtonPrefab.gameObject, tradeButtonContainer, 10);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
            Cancel();
    }

    public void Show(Unit user)
    {
        this.user = user;
        UIManager.main.BattleUI?.SetUIEnabled(false);
        UIManager.main.TopBarUI.SetTitleText("Program Trader", true);
        argumentIndex = 0;
        foreach(var button in tradeButtons)
        {
            button.Hide();
            tradeButtonPool.Release(button);
        }
        foreach(var button in tradeArguments)
        {
            button.Hide();
        }
        var programs = new List<Program>(PersistantData.main.inventory.AllRemovablePrograms);
        UIManager.main.ItemSelector.Show("Select Programs", programs, user, this);
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        HideUI();
        OnComplete();
    }

    private void HideUI()
    {
        UIManager.main.ItemSelector.Hide();
        gameObject.SetActive(false);
    }

    private void OnTradeLootComplete()
    {
        SaveManager.Save(SaveManager.State.Battle);
        Show(user);
    }

    private void DoTrade()
    {
        // Remove trade arguments from inventory
        var shell1 = tradeArguments[0].Program.Shell;
        var shell2 = tradeArguments[1].Program.Shell;
        foreach (var button in tradeArguments)
        {
            if (button.Program.Shell != null)
            {
                button.Program.Shell.Uninstall(button.Program);
            }
            else
            {
                PersistantData.main.inventory.RemoveProgram(button.Program, false);
            }
        }
        if (shell1 != null)
        {
            shell1.Compile();
        }
        if (shell2 != null && shell2 != shell1)
        {
            shell2.Compile();
        }
        HideUI();
        UIManager.main.TopBarUI.EndTempTitleText();
    }

    private void OnComplete()
    {
        UIManager.main.BattleUI?.SetUIEnabled(true);
        UIManager.main.TopBarUI.EndTempTitleText();
    }

    public bool TrySelectItem(object item)
    {
        if (argumentIndex >= tradeArguments.Count)
            return false;
        if (!(item is Program program))
            return false;
        tradeArguments[argumentIndex].Setup(program, user);
        if(++argumentIndex >= tradeArguments.Count)
        {
            TradeReady();
        }
        return true;
    }

    private void Cancel()
    {
        if(argumentIndex <= 0)
        {
            return;
        }
        CancelInternal(--argumentIndex);
    }

    public void Cancel(int index)
    {
        CancelInternal(index);
    }

    private void CancelInternal(int index)
    {
        var button = tradeArguments[index];
        UIManager.main.ItemSelector.ReturnItem(button.Program);
        if (index == 0)
        {
            if (tradeArguments[1].gameObject.activeSelf)
            {
                tradeArguments[0].Setup(tradeArguments[1].Program, user);
                tradeArguments[1].Hide();
                argumentIndex = 1;
            }
            else
            {
                button.Hide();
                argumentIndex = 0;
            }
        }
        else
        {
            button.Hide();
            argumentIndex = 1;
        }
        CancelTradeReady();
    }

    private void CancelTradeReady()
    {
        foreach(var button in tradeButtons)
        {
            button.Hide();
            tradeButtonPool.Release(button);
        }
    }

    private void TradeReady()
    {
        var prog1 = tradeArguments[0].Program;
        var prog2 = tradeArguments[1].Program;
        // Rarity Option
        int score = Score(prog1.Rarity) + Score(prog2.Rarity);
        if(score >= 10)
        {
            AddProgramDrop(rarePlusDrop);
        }
        else if(score >= 5)
        {
            AddProgramDrop(rareDrop);
        }
        else if(score >= 3)
        {
            AddProgramDrop(uncommonDrop);
        }
        else
        {
            AddProgramDrop(commonDrop);
        }
        // Color Option
        if(prog1.color == prog2.color && colorDict.TryGetValue(prog1.color, out var colorDrop))
        {
            AddProgramDrop(colorDrop);
        }
        if(ProgramFilters.HasAttributes(Program.Attributes.Gamble, prog1, prog2))
        {
            AddProgramDrop(gambleDrop);
        }
        // Money Options
        AddMoneyDrop(50);
    }

    private void AddProgramDrop(DropComponent<Program> drop)
    {
        var button = tradeButtonPool.Get();
        button.SetupAsLoot(drop.DisplayName, drop, null, 0, DoTrade, OnTradeLootComplete, true);
        button.transform.SetAsLastSibling();
        tradeButtons.Add(button);
    }

    private void AddMoneyDrop(int amount)
    {
        var button = tradeButtonPool.Get();
        button.SetupAsLoot($"${amount}", null, null, amount, DoTrade, OnTradeLootComplete, true);
        button.transform.SetAsLastSibling();
        tradeButtons.Add(button);
    }

    private int Score(Rarity rarity)
    {
        return rarity switch
        {
            Rarity.Common => 1,
            Rarity.Uncommon => 2,
            Rarity.Rare => 5,
            Rarity.Elite => 2,
            Rarity.Boss => 5,
            Rarity.Unique => 2,
            Rarity.PreInstall => 1,
            Rarity.Debug => 1,
            Rarity.SoulCore => 1,
            Rarity.Fusion => 3,
            Rarity.Shop => 1,
            Rarity.ShopShellUpgrade => 2,
            _ => 0,
        };
    }

    [Serializable]
    private class ColorDropData
    {
        public Program.Color color;
        public DropComponent<Program> dropComponent;
    }
}
