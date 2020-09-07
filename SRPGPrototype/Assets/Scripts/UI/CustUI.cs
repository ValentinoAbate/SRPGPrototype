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
    public Canvas uiCanvas;
    public CustCursor cursor;

    [Header("Shell Menu UI")]
    public ShellButton equippedShellButton;
    public GameObject shellButtonPrefab;
    public GameObject shellButtonContainer;
    public Button exitToBattleButton;
    public TextMeshProUGUI shellHpNumberText;
    public TextMeshProUGUI shellLvNumberText;
    public TextMeshProUGUI shellCapNumberText;
    public TextMeshProUGUI shellNextNumberText;

    [Header("Cust UI")]
    public CustGrid grid;
    public PatternDisplayUI heldProgramUI;
    public GameObject programPatternIconPrefab;

    [Header("Program Button UI")]
    public GameObject programButtonPrefab;
    public GameObject programButtonContainer;


    [Header("Program Description UI")]
    public ProgramDescriptionUI programDesc;

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

    private void LateUpdate()
    {
        if (heldProgramUI.isActiveAndEnabled)
        {
            Vector2 viewportPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            var rectTransform = heldProgramUI.GetComponent<RectTransform>();
            var offset = Vector2.right * 0.0005f;
            rectTransform.anchorMin = viewportPos + offset;
            rectTransform.anchorMax = viewportPos + offset;
        }
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

    public void UpdateCompileButtonColor()
    {
        compileButton.image.color = grid.Shell.Compiled ? Color.green : Color.red;
    }

    public void UpdateShellPropertyUI()
    {
        var shell = grid.Shell;
        int level = shell.Level;
        shellHpNumberText.text = shell.Stats.HP.ToString() + "/" + shell.Stats.MaxHP;
        shellLvNumberText.text = level.ToString();
        shellCapNumberText.text = shell.Capacity.ToString();
        shellNextNumberText.text = level == shell.MaxLevel ? "N/A" : shell.CapacityThresholds[level + 1].ToString();
    }

    public void EnterCust(Shell s)
    {
        grid.Shell = s;
        UpdateCompileButtonColor();
        programButtonContainer.transform.DestroyAllChildren();
        foreach (var program in inventory.Programs)
        {
            var pButton = Instantiate(programButtonPrefab, programButtonContainer.transform);
            var progButtonComponent = pButton.GetComponent<ProgramButton>();
            progButtonComponent.Initialize(program, this);
        }
        shellMenuUI.SetActive(false);
        custUI.SetActive(true);
        // Set shell property display values
        UpdateShellPropertyUI();
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
        heldProgramUI.Show(p.shape, programPatternIconPrefab, p.ColorValue);
        heldProgramUI.gameObject.SetActive(true);
        selectedProgram = p;
        cursor.OnCancel = () => CancelProgramPlacement(p, button);
        cursor.OnClick = (pos) => PlaceProgam(button, p, pos);
    }

    public void CancelProgramPlacement(Program p , ProgramButton b)
    {
        b.Cancel();
        heldProgramUI.Hide();
        heldProgramUI.gameObject.SetActive(false);
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
                var button = Instantiate(programButtonPrefab, programButtonContainer.transform);
                var progButtonComponent = button.GetComponent<ProgramButton>();
                progButtonComponent.Initialize(prog, this);
                grid.Shell.Uninstall(prog, prog.Pos);
                grid.Remove(prog);
                inventory.AddProgram(prog);
                UpdateCompileButtonColor();
                progButtonComponent.button.onClick.Invoke();
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
            heldProgramUI.Hide();
            heldProgramUI.gameObject.SetActive(false);
            UpdateCompileButtonColor();
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
        programDesc.Show(p);
        programDesc.gameObject.SetActive(true);
    }

    public void HideProgramDescriptionWindow(bool fromGrid)
    {
        if (fromGrid != descEnabledFromGrid)
            return;
        programDesc.gameObject.SetActive(false);
    }

    public void Compile()
    {
        int level = grid.Shell.Level;
        grid.Shell.Compile();
        if(grid.Shell.Level != level)
        {
            grid.ResetShell();
        }
        UpdateCompileButtonColor();
        UpdateShellPropertyUI();
    }

    public void UninstallAll()
    {
        var installs = new List<Shell.InstalledProgram>(grid.Shell.Programs);
        foreach(var install in installs)
        {
            var prog = install.program;
            if(!prog.attributes.HasFlag(Program.Attributes.Fixed))
            {
                var button = Instantiate(programButtonPrefab, programButtonContainer.transform);
                var progButtonComponent = button.GetComponent<ProgramButton>();
                progButtonComponent.Initialize(prog, this);
                grid.Shell.Uninstall(prog, prog.Pos);
                grid.Remove(prog);
                inventory.AddProgram(prog);
            }
        }
        UpdateCompileButtonColor();
    }

    #endregion
}
