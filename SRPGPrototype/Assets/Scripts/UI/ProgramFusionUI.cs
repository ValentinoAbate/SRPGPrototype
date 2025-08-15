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
    private IReadOnlyList<Program> fusions = System.Array.Empty<Program>();
    private int argumentIndex = 0;
    private Unit user;

    public void Show(Unit user)
    {
        UIManager.main.BattleUI?.SetUIEnabled(false);
        UIManager.main.TopBarUI.SetTitleText("Program Fusion", true);
        this.user = user;
        argumentIndex = 0;
        foreach(var button in previewButtons)
        {
            button.Hide();
        }
        foreach(var button in fusionArguments)
        {
            button.Hide();
        }
        UIManager.main.ItemSelector.Show("Select a Program", PersistantData.main.inventory.Programs, user, this);
        gameObject.SetActive(true);
        confirmButton.SetInteractable(false);
    }

    public void Hide()
    {
        HideUI();
        OnComplete();
    }

    private void OnComplete()
    {
        UIManager.main.BattleUI?.SetUIEnabled(true);
        UIManager.main.TopBarUI.EndTempTitleText();
    }

    private void HideUI()
    {
        UIManager.main.ItemSelector.Hide();
        gameObject.SetActive(false);
    }

    public void Confirm()
    {
        // Remove fusion arguments from inventory
        foreach (var button in fusionArguments)
        {
            PersistantData.main.inventory.RemoveProgram(button.Program, false);
        }
        // Generate Program loot
        var progDraws = new LootData<Program>();
        progDraws.Add(fusions, "Fused Program", 0);
        HideUI();
        PersistantData.main.loot.UI.ShowUI(PersistantData.main.inventory, progDraws, null, System.Array.Empty<LootUI.MoneyData>(), OnComplete);
    }

    public bool TrySelectItem(object item)
    {
        if (argumentIndex >= fusionArguments.Count)
            return false;
        if (!(item is Program program))
            return false;
        fusionArguments[argumentIndex].Setup(program, null, user);
        if(++argumentIndex >= fusionArguments.Count)
        {
            FusionReady();
        }
        return true;
    }

    private void FusionReady()
    {
        fusions = programFuser.GetFusions(previewProgramContainer, fusionArguments[0].Program, fusionArguments[1].Program, int.MaxValue, int.MaxValue);
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
}
