using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustUI : MonoBehaviour
{
    public bool EquippedShellCompiled
    {
        get => exitToBattleButton.interactable;
        private set
        {
            exitToBattleButton.interactable = value;
        }
    }

    public GameObject shellMenuUI;
    public GameObject custUI;
    public CustCursor cursor;

    [Header("Shell Menu UI")]
    public ShellButton equippedShellButton;
    public GameObject shellButtonPrefab;
    public GameObject shellButtonContainer;
    public Button exitToBattleButton;

    [Header("Cust UI")]
    public CustGrid grid;
    [Header("Program Button UI")]
    public GameObject programButtonPrefab;
    public GameObject programButtonContainer;

    [Header("Program Description UI")]
    public GameObject programDescWindow;
    public TextMeshProUGUI programNameText;
    public TextMeshProUGUI programDescText;
    public TextMeshProUGUI programAttrText;

    [Header("Compile UI")]
    public Button compileButton;
    public Button exitCustButton;

    private Inventory inventory;
    private ProgramButton pButton;
    private Program selectedProgram;

    private void Start()
    {
        HideProgramDescriptionWindow(descEnabledFromGrid);
        inventory = PersistantData.main.inventory;
        EnterShellMenu();
    }

    #region Shell Menu UI

    public void EnterShellMenu()
    {
        EquippedShellCompiled = inventory.EquippedShell.Compiled;
        cursor.NullAllActions();
        GenerateShellButtons();
        shellMenuUI.SetActive(true);
        custUI.SetActive(false);
    }

    public void EquipShell(Shell s)
    {
        inventory.EquippedShell = s;
        EquippedShellCompiled = s.Compiled;
        GenerateShellButtons();
    }

    private void GenerateShellButtons()
    {
        shellButtonContainer.transform.DestroyAllChildren();
        foreach (var shell in inventory.Shells)
        {
            if (shell == inventory.EquippedShell)
            {
                equippedShellButton.Initialize(shell, this, true);
                continue;
            }
            var sButton = Instantiate(shellButtonPrefab, shellButtonContainer.transform);
            var progButtonComponent = sButton.GetComponent<ShellButton>();
            progButtonComponent.Initialize(shell, this);
        }
    }

    #endregion

    #region Cust UI

    public void EnterCust(Shell s)
    {
        grid.Shell = s;
        programButtonContainer.transform.DestroyAllChildren();
        foreach (var program in inventory.Programs)
        {
            var pButton = Instantiate(programButtonPrefab, programButtonContainer.transform);
            var progButtonComponent = pButton.GetComponent<ProgramButton>();
            progButtonComponent.Initialize(program, this);
        }
        shellMenuUI.SetActive(false);
        custUI.SetActive(true);
        // Set cursor properties
        cursor.NullAllActions();
        cursor.OnHighlight = (pos) => HighlightProgram(pos);
        cursor.OnCancel = () => PickupProgramFromGrid(GetMouseGridPos());
    }

    public void HighlightProgram(Vector2Int pos)
    {
        if (grid.IsLegal(pos) && !grid.IsEmpty(pos))
        {
            ShowProgramDescriptionWindow(grid.Get(pos), true);
        }
        else
        {
            HideProgramDescriptionWindow(true);

        }
    }

    public void PickupProgramFromButton(ProgramButton button, Program p)
    {
        if (pButton != null)
            pButton.Cancel();
        pButton = button;
        selectedProgram = p;
        cursor.OnCancel = () => CancelProgramPlacement(p, button);
        cursor.OnClick = (pos) => PlaceProgam(button, p, pos);
    }

    public void CancelProgramPlacement(Program p , ProgramButton b)
    {
        b.Cancel();
        // Set cursor properties
        cursor.OnCancel = () => PickupProgramFromGrid(GetMouseGridPos());
        cursor.OnClick = null;
    }

    public void PickupProgramFromGrid(Vector2Int pos)
    {
        if (grid.IsLegal(pos))
        {
            var prog = grid.Get(pos);
            if (prog != null && !prog.attributes.HasFlag(Program.Attributes.Fixed))
            {
                var pButton = Instantiate(programButtonPrefab, programButtonContainer.transform);
                var progButtonComponent = pButton.GetComponent<ProgramButton>();
                progButtonComponent.Initialize(prog, this);
                grid.Shell.Uninstall(prog, prog.Pos);
                grid.Remove(prog);
                inventory.AddProgram(prog);
            }
        }
    }

    public void PlaceProgam(ProgramButton button, Program p, Vector2Int pos)
    {
        if (grid.IsLegal(pos) && grid.Add(pos, selectedProgram))
        {
            inventory.RemoveProgram(selectedProgram);
            grid.Shell.Install(selectedProgram, pos);
            selectedProgram = null;
            Destroy(pButton.gameObject);
            cursor.OnClick = null;
            cursor.OnCancel = () => PickupProgramFromGrid(GetMouseGridPos());
        }
    }

    private Vector2Int GetMouseGridPos()
    {
        return grid.GetPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    private bool descEnabledFromGrid = false;
    public void ShowProgramDescriptionWindow(Program p, bool fromGrid)
    {
        descEnabledFromGrid = fromGrid;
        programNameText.text = p.DisplayName;
        programDescText.text = p.Description;
        programAttrText.text = GetAttributesText(p);
        programDescWindow.SetActive(true);
    }

    private string GetAttributesText(Program p)
    {
        var attTexts = new List<string>();
        if (p.attributes.HasFlag(Program.Attributes.Fixed))
        {
            attTexts.Add("Fixed");
        }
        if(p.attributes.HasFlag(Program.Attributes.Transient))
        {
            var transientAttr = p.GetComponent<ProgramAttributeTransient>();
            string errorText = "Error: No Attribute Component found";
            attTexts.Add("Transient " + (transientAttr == null ? errorText : transientAttr.UsesLeft.ToString()));
        }
        if (attTexts.Count <= 0)
            return string.Empty;
        if (attTexts.Count == 1)
            return attTexts[0];
        return attTexts.Aggregate((s1, s2) => s1 + ", " + s2);
    }

    public void HideProgramDescriptionWindow(bool fromGrid)
    {
        if (fromGrid != descEnabledFromGrid)
            return;
        programDescWindow.SetActive(false);
    }

    public void Compile()
    {
        int level = grid.Shell.Level;
        grid.Shell.Compile();
        if(grid.Shell.Level != level)
        {
            grid.ResetShell();
        }
    }

    #endregion
}
