using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustUI : MonoBehaviour
{
    private const float buttonContentSize = 70;
    public bool EquippedShellCompiled
    {
        get => exitToBattleButton.interactable;
        private set
        {
            exitToBattleButton.interactable = value;
        }
    }

    [SerializeField] private GameObject shellMenuUI;
    [SerializeField] private GameObject custUI;
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private CustCursor cursor;

    [Header("Shell Menu UI")]
    [SerializeField] private ShellButton equippedShellButton;
    [SerializeField] private GameObject shellButtonPrefab;
    [SerializeField] private GameObject shellButtonContainer;
    [SerializeField] private Button exitToBattleButton;
    [SerializeField] private ShellInfoDisplayUI shellInfoDisplayUI;

    [Header("Cust UI")]
    [SerializeField] private CustGrid grid;
    [SerializeField] private PatternDisplayUI heldProgramUI;
    [SerializeField] private GameObject programPatternIconPrefab;

    [Header("Program Button UI")]
    [SerializeField] private GameObject programButtonPrefab;
    [SerializeField] private Transform programButtonContainer;
    [SerializeField] private RectTransform programButtonRect;

    [Header("Program Description UI")]
    [SerializeField] private CompileDataProxyUnit proxyUnit;

    [Header("Compile UI")]
    [SerializeField] private Button compileButton;
    [SerializeField] private Button exitCustButton;

    [Header("Preset UI")]
    [SerializeField] private PresetUI presetUI;

    public CustGrid Grid => grid;
    public Shell Shell => grid.Shell;
    private Inventory inventory;
    private ProgramButton pButton;

    private void Start()
    {
        HideProgramDescriptionWindow(descEnabledFromGrid);
        inventory = PersistantData.main.inventory;
        shellInfoDisplayUI.Initialize(LevelUp, LevelDown);
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
        if(inventory.EquippedShell == null)
        {
            EquippedShellCompiled = false;
        }
        else if (inventory.EquippedShell.HasSoulCore)
        {
            inventory.EquippedShell = null;
        }
        else
        {
            EquippedShellCompiled = inventory.EquippedShell.Compiled;
        }
        cursor.NullAllActions();
        GenerateShellButtons();
        shellMenuUI.SetActive(true);
        custUI.SetActive(false);
        UIManager.main.UpdateShellViewerController();
        UIManager.main.HideAllDescriptionUI();
        UIManager.main.TopBarUI.SetTitleText("Shell Customization");
        UIManager.main.TopBarUI.Show();
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
            if (shell == inventory.EquippedShell && shell != null)
            {
                equippedShellButton.Initialize(shell, this, true);
                continue;
            }
            var sButton = Instantiate(shellButtonPrefab, shellButtonContainer.transform);
            var progButtonComponent = sButton.GetComponent<ShellButton>();
            progButtonComponent.Initialize(shell, this);
        }
        equippedShellButton.gameObject.SetActive(inventory.EquippedShell != null);
    }

    #endregion

    #region Cust UI

    public void UpdateCompileButtonColor()
    {
        compileButton.image.color = Shell.Compiled ? Color.green : Color.red;
    }

    private void UpdateCompileData()
    {
        var compileData = Shell.GetCompileData();
        shellInfoDisplayUI.UpdateUI(Shell, compileData);
        proxyUnit.UpdateData(Shell.Stats.HP, compileData);
    }

    public void LevelUp()
    {
        if(Shell.Level < Shell.MaxLevel)
        {
            Shell.LevelUp();
            grid.ResetShell();
            UpdateCompileData();
            UpdateCompileButtonColor();
        }
    }

    public void LevelDown()
    {
        if (Shell.Level > 0)
        {
            if(Shell.CanLevelDown())
            {
                Shell.LevelDown();
                grid.ResetShell();
                UpdateCompileData();
                UpdateCompileButtonColor();
            }
            else
            {
                Debug.LogWarning("Can't Level Down: Programs in level-down area");
            }
        }
    }

    public void EnterCust(Shell s)
    {
        grid.Shell = s;
        UpdateCompileButtonColor();
        // Initialize the program buttons
        InitializeProgramButtons();
        shellMenuUI.SetActive(false);
        custUI.SetActive(true);
        // Set shell property display values
        UpdateCompileData();
        // Initialize Preset UI
        presetUI.Initialize(s, grid, this);
        // Set cursor properties
        cursor.NullAllActions();
        cursor.OnHighlight = HighlightProgram;
        cursor.OnUnHighlight = UnHighlightProgram;
        cursor.OnCancel = () => PickupProgramFromGrid(GetMouseGridPos());
        UIManager.main.TopBarUI.Hide();
        UIManager.main.HideAllDescriptionUI();
    }

    public void InitializeProgramButtons()
    {
        programButtonContainer.DestroyAllChildren();
        foreach (var program in inventory.Programs)
        {
            AddProgramButton(program);
        }
    }

    private ProgramButton AddProgramButton(Program p)
    {
        var pButton = Instantiate(programButtonPrefab, programButtonContainer);
        var progButtonComponent = pButton.GetComponent<ProgramButton>();
        progButtonComponent.Initialize(p, this);
        programButtonRect.sizeDelta = new Vector2(programButtonRect.sizeDelta.x, programButtonContainer.childCount * buttonContentSize);
        return progButtonComponent;
    }

    public void HighlightProgram(Vector2Int pos)
    {
        if (grid.IsLegal(pos) && !grid.IsEmpty(pos))
        {
            var prog = grid.Get(pos);
            prog.Highlight();
            ShowProgramDescriptionWindow(prog, true);
        }
        else
        {
            HideProgramDescriptionWindow(true);
        }
    }

    public void UnHighlightProgram(Vector2Int pos)
    {
        if (grid.IsLegal(pos) && !grid.IsEmpty(pos))
        {
            grid.Get(pos).UnHighlight();
        }
    }

    public void PickupProgramFromButton(ProgramButton button, Program p)
    {
        if (pButton != null)
            pButton.Cancel();
        pButton = button;
        heldProgramUI.Show(p.shape, programPatternIconPrefab, p.ColorValue);
        heldProgramUI.gameObject.SetActive(true);
        cursor.OnCancel = () => CancelProgramPlacement(p, button);
        cursor.OnClick = (pos) => PlaceProgram(button, p, pos);
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
                Shell.Uninstall(prog);
                grid.Remove(prog);
                inventory.AddProgram(prog);
                UpdateCompileData();
                UpdateCompileButtonColor();
                var progButtonComponent = AddProgramButton(prog);
                progButtonComponent.button.onClick.Invoke();
            }
        }
    }

    public void PlaceProgram(ProgramButton button, Program p, Vector2Int pos)
    {
        if (grid.IsLegal(pos) && grid.CanAdd(pos, p))
        {
            if(ProgramFilters.HasAttributes(p, Program.Attributes.Fixed))
            {
                void OnConfirmationPopupComplete(bool success)
                {
                    if (success)
                    {
                        PlaceProgramInternal(p, pos);
                    }
                    else
                    {
                        cursor.OnCancel = () => CancelProgramPlacement(p, button);
                        cursor.OnClick = (pos) => PlaceProgram(button, p, pos);
                        heldProgramUI.Show(p.shape, programPatternIconPrefab, p.ColorValue);
                        heldProgramUI.gameObject.SetActive(true);
                    }
                }
                cursor.OnClick = null;
                cursor.OnCancel = null;
                heldProgramUI.Hide();
                heldProgramUI.gameObject.SetActive(false);
                PopupManager.main.ShowConfirmationPopup(OnConfirmationPopupComplete, "Warning!", $"{p.DisplayName} is fixed. You will not be able to remove it after installing it.", "Install");
            }
            else
            {
                PlaceProgramInternal(p, pos);
            }
        }
    }

    private void PlaceProgramInternal(Program p, Vector2Int pos)
    {
        grid.Add(pos, p);
        inventory.RemoveProgram(p);
        Shell.Install(p, pos);
        Destroy(pButton.gameObject);
        programButtonRect.sizeDelta = new Vector2(programButtonRect.sizeDelta.x, programButtonContainer.childCount * buttonContentSize);
        cursor.OnClick = null;
        cursor.OnCancel = () => PickupProgramFromGrid(GetMouseGridPos());
        heldProgramUI.Hide();
        heldProgramUI.gameObject.SetActive(false);
        UpdateCompileData();
        UpdateCompileButtonColor();
    }

    private Vector2Int GetMouseGridPos()
    {
        return grid.GetPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    private bool descEnabledFromGrid = false;
    public void ShowProgramDescriptionWindow(Program p, bool fromGrid)
    {
        descEnabledFromGrid = fromGrid;
        UIManager.main.ProgramDescriptionUI.Show(p, proxyUnit);
    }

    public void HideProgramDescriptionWindow(bool fromGrid)
    {
        if (fromGrid != descEnabledFromGrid)
            return;
        UIManager.main.ProgramDescriptionUI.Hide();
    }

    public void Compile()
    {
        Shell.Compile();
        UpdateCompileButtonColor();
        UpdateCompileData();
    }

    public void UninstallAll()
    {
        var installs = new List<Shell.InstalledProgram>(Shell.Programs);
        foreach(var install in installs)
        {
            var prog = install.program;
            if(!prog.attributes.HasFlag(Program.Attributes.Fixed))
            {
                AddProgramButton(prog);
                Shell.Uninstall(prog);
                grid.Remove(prog);
                inventory.AddProgram(prog);
            }
        }
        
        UpdateCompileButtonColor();
        UpdateCompileData();
        presetUI.Clear();
    }

    #endregion
}
