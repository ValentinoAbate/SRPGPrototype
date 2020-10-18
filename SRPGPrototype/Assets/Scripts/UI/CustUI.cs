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

    public GameObject shellMenuUI;
    public GameObject custUI;
    public Canvas uiCanvas;
    public CustCursor cursor;

    [Header("Shell Menu UI")]
    public ShellButton equippedShellButton;
    public GameObject shellButtonPrefab;
    public GameObject shellButtonContainer;
    public Button exitToBattleButton;
    public ShellDescriptionUI shellDescriptionUI;

    [Header("Cust UI")]
    public CustGrid grid;
    public PatternDisplayUI heldProgramUI;
    public GameObject programPatternIconPrefab;

    [Header("Shell Level Info Panel")]
    public TextMeshProUGUI shellHpNumberText;
    public TextMeshProUGUI shellLvNumberText;
    public TextMeshProUGUI shellCapNumberText;
    public Button levelUpButton;
    public Button levelDownButton;

    [Header("Program Button UI")]
    public GameObject programButtonPrefab;
    public Transform programButtonContainer;
    public RectTransform programButtonRect;


    [Header("Program Description UI")]
    public ProgramDescriptionUI programDesc;

    [Header("Compile UI")]
    public Button compileButton;
    public Button exitCustButton;

    [Header("Preset UI")]
    public TextMeshProUGUI presetNameText;
    public Button savePresetButton;
    public TextMeshProUGUI[] preset1ButtonTexts;
    public TextMeshProUGUI[] preset2ButtonTexts;
    public TextMeshProUGUI[] preset3ButtonTexts;
    public Button[] loadButtons;
    public Button[] saveNewButtons;

    private Shell Shell => grid.Shell;
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
        shellDescriptionUI.Hide();
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

    public void UpdateShellPropertyUI()
    {
        int level = Shell.Level;
        var compileData = Shell.GetCompileData(out List<Action> newActions);
        shellHpNumberText.text = Mathf.Clamp(Shell.Stats.HP, 0, compileData.stats.MaxHP).ToString() + "/" + compileData.stats.MaxHP;
        shellLvNumberText.text = level == Shell.MaxLevel ? "Max" : level.ToString();
        shellCapNumberText.text = compileData.capacity.ToString() + "/" + Shell.CapacityThresholds[level].ToString();
        levelDownButton.interactable = level > 0;
        levelUpButton.interactable = level < Shell.MaxLevel;
        newActions.ForEach((a) => Destroy(a.gameObject));
    }

    public void LevelUp()
    {
        if(Shell.Level < Shell.MaxLevel)
        {
            Shell.LevelUp();
            grid.ResetShell();
            UpdateShellPropertyUI();
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
                UpdateShellPropertyUI();
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
        UpdateShellPropertyUI();
        // Initialize Preset UI
        InitializePresetUI();
        // Set cursor properties
        cursor.NullAllActions();
        cursor.OnHighlight = (pos) => HighlightProgram(pos);
        cursor.OnCancel = () => PickupProgramFromGrid(GetMouseGridPos());
    }

    private void InitializeProgramButtons()
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
                Shell.Uninstall(prog, prog.Pos);
                grid.Remove(prog);
                inventory.AddProgram(prog);
                UpdateShellPropertyUI();
                UpdateCompileButtonColor();
                var progButtonComponent = AddProgramButton(prog);
                progButtonComponent.button.onClick.Invoke();
            }
        }
    }

    public void PlaceProgam(ProgramButton button, Program p, Vector2Int pos)
    {
        if (grid.IsLegal(pos) && grid.Add(pos, selectedProgram))
        {
            inventory.RemoveProgram(selectedProgram);
            Shell.Install(selectedProgram, pos);
            selectedProgram = null;
            Destroy(pButton.gameObject);
            programButtonRect.sizeDelta = new Vector2(programButtonRect.sizeDelta.x, programButtonContainer.childCount * buttonContentSize);
            cursor.OnClick = null;
            cursor.OnCancel = () => PickupProgramFromGrid(GetMouseGridPos());
            heldProgramUI.Hide();
            heldProgramUI.gameObject.SetActive(false);
            UpdateShellPropertyUI();
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
        Shell.Compile();
        UpdateCompileButtonColor();
        UpdateShellPropertyUI();
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
                Shell.Uninstall(prog, prog.Pos);
                grid.Remove(prog);
                inventory.AddProgram(prog);
            }
        }
        
        UpdateCompileButtonColor();
        UpdateShellPropertyUI();
        SetLoadedPresetIndex(PresetManager.noLoadedPreset);
    }

    #region Preset UI

    private int loadedPresetIndex;
    private Shell.Preset[] presets;
    private Shell.Preset LoadedPreset => presets[loadedPresetIndex];
    private string saveAsNewPresetName = Shell.Preset.defaultName;
    private int saveAsNewPresetInd = 0;

    private void InitializePresetUI()
    {
        var presetManager = PersistantData.main.presetManager;
        presets = presetManager.GetPresets(Shell);
        SetLoadedPresetIndex(presetManager.GetLoadedPreset(Shell), false);
        InitializePresetButtons();
    }

    private void SetLoadedPresetIndex(int index, bool updatePresetManager = true)
    {
        loadedPresetIndex = index;
        if(updatePresetManager)
        {
            var presetManager = PersistantData.main.presetManager;
            presetManager.SetLoadedPreset(Shell, index);
        }
        if (loadedPresetIndex == PresetManager.noLoadedPreset)
        {
            presetNameText.text = "Preset: None";
            savePresetButton.interactable = false;
        }
        else
        {
            presetNameText.text = "Preset: " + LoadedPreset.DisplayName;
            savePresetButton.interactable = true;
        }
    }

    private void InitializePresetButtons()
    {
        foreach (var text in preset1ButtonTexts)
            text.text = presets[0]?.DisplayName ?? Shell.Preset.defaultName;
        foreach (var text in preset2ButtonTexts)
            text.text = presets[1]?.DisplayName ?? Shell.Preset.defaultName;
        foreach (var text in preset3ButtonTexts)
            text.text = presets[2]?.DisplayName ?? Shell.Preset.defaultName;
        for (int i = 0; i < loadButtons.Length; ++i)
        {
            loadButtons[i].interactable = presets[i] != null;
        }
    }

    private Shell.Preset CreatePreset(string name)
    {
        var preset = new Shell.Preset() { DisplayName = name };
        preset.Level = Shell.Level;
        preset.Programs = new List<Shell.InstalledProgram>(Shell.Programs);
        return preset;
    }

    public void SavePreset()
    {
        presets[loadedPresetIndex] = CreatePreset(LoadedPreset.DisplayName);
    }

    public void SetSaveAsNewPresetName(string name)
    {
        saveAsNewPresetName = name;
        foreach (var button in saveNewButtons)
            button.interactable = !string.IsNullOrWhiteSpace(name);
    }

    public void SetSaveAsNewPresetInd(int ind)
    {
        saveAsNewPresetInd = ind;
        foreach (var button in saveNewButtons)
            button.interactable = false;
    }

    public void SaveNewPreset()
    {
        presets[saveAsNewPresetInd] = CreatePreset(saveAsNewPresetName);
        InitializePresetButtons();
    }

    public void SaveNewPresetAndSetLoaded()
    {
        SaveNewPreset();
        SetLoadedPresetIndex(saveAsNewPresetInd);
    }

    public void LoadPreset(int index)
    {
        var preset = presets[index];
        if(index == PresetManager.noLoadedPreset)
        {
            savePresetButton.interactable = false;
            UninstallAll();
            return;
        }
        var installs = new List<Shell.InstalledProgram>(Shell.Programs);
        foreach (var install in installs)
        {
            var prog = install.program;
            Shell.Uninstall(prog, prog.Pos);
            grid.Remove(prog);
        }
        // Set shell to proper Level
        if(Shell.Level != preset.Level)
        {
            Shell.SetLevel(preset.Level);
            grid.ResetShell();
        }
        // Add the programs to shell from the inventory
        foreach(var prog in preset.Programs)
        {
            int progInd = installs.FindIndex((i) => i.program == prog.program);
            // Add from uninstalled programs if applicable
            if(progInd != -1)
            {
                Shell.Install(prog.program, prog.location);
                installs.RemoveAt(progInd);
                grid.Add(prog.location, prog.program);
            }
            else if(inventory.Programs.Contains(prog.program))
            {
                inventory.RemoveProgram(prog.program);
                Shell.Install(prog.program, prog.location);
                grid.Add(prog.location, prog.program);
            }
        }
        // Add any programs that weren't reinstalled back to the inventory
        foreach(var prog in installs)
        {
            inventory.AddProgram(prog.program);
        }
        // Reinitialize the program button UI
        InitializeProgramButtons();
        SetLoadedPresetIndex(index);
        UpdateCompileButtonColor();
    }

    #endregion

    #endregion
}
