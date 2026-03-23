using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramTraderUI : MonoBehaviour, ISelectableItemHandler
{
    [SerializeField] private Transform previewButtonContainer;
    [SerializeField] private ProgramItemButton previewButtonPrefab;
    [SerializeField] private List<ProgramItemButton> tradeArguments;

    private readonly List<ProgramItemButton> previewButtons = new List<ProgramItemButton>();
    private int argumentIndex = 0;
    private Unit user;

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
        foreach(var button in previewButtons)
        {
            button.Hide();
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

    private void SelectTrade(int index)
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

        // TODO: award trade item
        SaveManager.Save(SaveManager.State.Battle);
        Hide();
    }

    private void OnComplete()
    {
        UIManager.main.BattleUI?.SetUIEnabled(true);
        UIManager.main.TopBarUI.EndTempTitleText();
        // TODO: cleanup
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
        foreach(var button in previewButtons)
        {
            button.Hide();
        }
        // TODO: clear offers
    }

    private void TradeReady()
    {
        // TODO: generate and show options
    }
}
