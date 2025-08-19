using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramFusionUI : MonoBehaviour, ISelectableItemHandler
{
    [SerializeField] private Transform previewButtonContainer;
    [SerializeField] private ProgramItemButton previewButtonPrefab;
    [SerializeField] private Transform previewProgramContainer;
    [SerializeField] private List<ProgramItemButton> fusionArguments;
    [SerializeField] private ProgramFuser programFuser;
    [SerializeField] private CostDisplay costDisplay;

    private readonly List<ProgramItemButton> previewButtons = new List<ProgramItemButton>();
    private readonly List<Program> fusions = new List<Program>();
    private int argumentIndex = 0;
    private int selectedFusion = 0;
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
        selectedFusion = 0;
        fusions.Clear();
        foreach(var button in previewButtons)
        {
            button.Hide();
        }
        foreach(var button in fusionArguments)
        {
            button.Hide();
        }
        var programs = new List<Program>(PersistantData.main.inventory.AllRemovablePrograms);
        UIManager.main.ItemSelector.Show("Select Programs", programs, user, this);
        gameObject.SetActive(true);
        costDisplay.Setup(cost);
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

    private void SelectFusion(int index)
    {
        selectedFusion = index;
        PopupManager.main.ShowInputPopup(OnNamingComplete, null, 16, false, "Name Your Creation");
    }

    private void OnNamingComplete(string programName)
    {
        // Finalize Fusion
        var program = programFuser.FusePrograms(transform, fusionArguments[0].Program, fusionArguments[1].Program, fusions[selectedFusion].shape);
        // Name program
        program.SetDisplayName(programName);
        program.SetActionNames(programName);
        // Add program to inventory
        PersistantData.main.inventory.AddProgram(program);
        // Remove fusion arguments from inventory
        var shell1 = fusionArguments[0].Program.Shell;
        var shell2 = fusionArguments[1].Program.Shell;
        foreach (var button in fusionArguments)
        {
            if(button.Program.Shell != null)
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
        if(shell2 != null && shell2 != shell1)
        {
            shell2.Compile();
        }
        // Spend cost
        PersistantData.main.inventory.Money -= cost;
        Hide();
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
        ClearFusions();
    }

    private void FusionReady()
    {
        bool canAfford = PersistantData.main.inventory.CanAfford(cost);
        fusions.Clear();
        fusions.AddRange(programFuser.GetFusionPreviews(previewProgramContainer, fusionArguments[0].Program, fusionArguments[1].Program, int.MaxValue, int.MaxValue));
        for (int i = 0; i < fusions.Count; ++i)
        {
            if (i >= previewButtons.Count)
            {
                previewButtons.Add(Instantiate(previewButtonPrefab, previewButtonContainer));
            }
            if (canAfford)
            {
                int index = i;
                void Select()
                {
                    SelectFusion(index);
                }
                previewButtons[i].Setup(fusions[i], Select, user);
            }
            else
            {
                previewButtons[i].Setup(fusions[i], null, user);
            }
        }
        for (int i = fusions.Count; i < previewButtons.Count; i++)
        {
            previewButtons[i].Hide();
        }
    }

    private void ClearFusions()
    {
        fusions.Clear();
        previewProgramContainer.DestroyAllChildren();
    }
}
