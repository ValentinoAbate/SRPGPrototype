using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramFusionUI : MonoBehaviour, ISelectableItemHandler
{
    [SerializeField] private Transform previewButtonContainer;
    [SerializeField] private ProgramItemButton previewButtonPrefab;
    [SerializeField] private Transform previewProgramContainer;
    [SerializeField] private List<ProgramItemButton> fusionArguments;
    [SerializeField] private CostButton confirmButton;
    [SerializeField] private ProgramFuser programFuser;

    private readonly List<ProgramItemButton> previewButtons = new List<ProgramItemButton>();
    private readonly List<Program> fusions = new List<Program>();
    private int argumentIndex = 0;
    private Unit user;
    private int cost;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
            Cancel();
    }

    public void Show(Unit user, int cost)
    {
        this.user = user;
        this.cost = cost;
        UIManager.main.BattleUI?.SetUIEnabled(false);
        UIManager.main.TopBarUI.SetTitleText("Program Fusion", true);
        argumentIndex = 0;
        fusions.Clear();
        foreach(var button in previewButtons)
        {
            button.Hide();
        }
        foreach(var button in fusionArguments)
        {
            button.Hide();
        }
        UIManager.main.ItemSelector.Show("Select Programs", PersistantData.main.inventory.Programs, user, this);
        gameObject.SetActive(true);
        confirmButton.SetCost(cost);
        confirmButton.SetInteractable(false);
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

    public void Confirm()
    {
        PopupManager.main.ShowInputPopup(OnNamingComplete, null, 16, false, "Name Your Creation");
    }

    private void OnNamingComplete(string programName)
    {
        // Name program
        foreach(var program in fusions)
        {
            program.SetDisplayName(programName);
            program.SetActionNames(programName);
        }
        // Remove fusion arguments from inventory
        foreach (var button in fusionArguments)
        {
            PersistantData.main.inventory.RemoveProgram(button.Program, true);
        }
        // Spend cost
        PersistantData.main.inventory.Money -= cost;
        // Generate Program loot
        var progDraws = new LootData<Program>();
        progDraws.Add(fusions, "Fused Program", 0);
        HideUI();
        PersistantData.main.loot.UI.ShowUI(PersistantData.main.inventory, progDraws, null, System.Array.Empty<LootUI.MoneyData>(), OnComplete);
    }

    private void OnComplete()
    {
        UIManager.main.BattleUI?.SetUIEnabled(true);
        UIManager.main.TopBarUI.EndTempTitleText();
        ClearFusions();
    }

    public bool TrySelectItem(object item)
    {
        if (argumentIndex >= fusionArguments.Count)
            return false;
        if (!(item is Program program))
            return false;
        fusionArguments[argumentIndex].Setup(program, user);
        if(++argumentIndex >= fusionArguments.Count)
        {
            FusionReady();
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
        var button = fusionArguments[index];
        UIManager.main.ItemSelector.ReturnItem(button.Program);
        if (index == 0)
        {
            if (fusionArguments[1].gameObject.activeSelf)
            {
                fusionArguments[0].Setup(fusionArguments[1].Program, user);
                fusionArguments[1].Hide();
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
        CancelFusionReady();
    }

    private void CancelFusionReady()
    {
        foreach(var button in previewButtons)
        {
            button.Hide();
        }
        confirmButton.SetInteractable(false);
        ClearFusions();
    }

    private void FusionReady()
    {
        fusions.Clear();
        fusions.AddRange(programFuser.GetFusions(previewProgramContainer, fusionArguments[0].Program, fusionArguments[1].Program, int.MaxValue, int.MaxValue));
        for (int i = 0; i < fusions.Count; ++i)
        {
            if (i >= previewButtons.Count)
            {
                previewButtons.Add(Instantiate(previewButtonPrefab, previewButtonContainer));
            }
            previewButtons[i].Setup(fusions[i], null, user);
        }
        for (int i = fusions.Count; i < previewButtons.Count; i++)
        {
            previewButtons[i].Hide();
        }
        confirmButton.SetInteractable(true);
    }

    private void ClearFusions()
    {
        fusions.Clear();
        previewProgramContainer.DestroyAllChildren();
    }
}
