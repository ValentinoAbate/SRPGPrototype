using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PresetUI : MonoBehaviour
{
    [Header("Preset UI")]
    [SerializeField] private TextMeshProUGUI presetNameText;
    [SerializeField] private Button savePresetButton;
    [SerializeField] private TextMeshProUGUI[] preset1ButtonTexts;
    [SerializeField] private TextMeshProUGUI[] preset2ButtonTexts;
    [SerializeField] private TextMeshProUGUI[] preset3ButtonTexts;
    [SerializeField] private GameObject loadMenu;
    [SerializeField] private Button[] loadButtons;
    [SerializeField] private GameObject saveNewMenu;
    [SerializeField] private Button[] saveNewButtons;
    [SerializeField] private GameObject newPresetNameMenu;
    [SerializeField] private TMP_InputField newPresetInput;


    private Shell Shell { get; set; }
    private Inventory Inventory => PersistantData.main.inventory;
    private CustGrid Grid { get; set; }
    private CustUI CustUI { get; set; }

    private int loadedPresetIndex;
    private Shell.Preset[] presets;
    private Shell.Preset LoadedPreset => presets[loadedPresetIndex];
    private string saveAsNewPresetName = Shell.Preset.defaultName;
    private int saveAsNewPresetInd = 0;

    public void Initialize(Shell s, CustGrid grid, CustUI ui)
    {
        Shell = s;
        Grid = grid;
        CustUI = ui;
        var presetManager = PersistantData.main.presetManager;
        presets = presetManager.GetPresets(Shell);
        EnterMainMenu();
        SetLoadedPresetIndex(presetManager.GetLoadedPreset(Shell), false);
        InitializePresetButtons();
    }

    private void SetLoadedPresetIndex(int index, bool updatePresetManager = true)
    {
        loadedPresetIndex = index;
        if (updatePresetManager)
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

    private List<Shell.InstalledProgram> ValidInstalls()
    {
        var installs = new List<Shell.InstalledProgram>(Shell.Programs.Count);
        foreach (var prog in Shell.Programs)
        {
            if (!prog.program.attributes.HasFlag(Program.Attributes.Fixed))
            {
                installs.Add(prog);
            }
        }
        return installs;
    }

    private Shell.Preset CreatePreset(string name)
    {
        var preset = new Shell.Preset() { DisplayName = name };
        preset.Level = Shell.Level;
        preset.Programs = ValidInstalls();
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
        if (index == PresetManager.noLoadedPreset)
        {
            savePresetButton.interactable = false;
            CustUI.UninstallAll();
            return;
        }
        var installs = ValidInstalls();
        foreach (var install in installs)
        {
            var prog = install.program;
            Shell.Uninstall(prog);
            Grid.Remove(prog);
        }
        // Set shell to proper Level
        if (Shell.Level != preset.Level)
        {
            Shell.SetLevel(preset.Level);
            Grid.ResetShell();
        }
        // Add the programs to shell from the inventory
        foreach (var prog in preset.Programs)
        {
            int progInd = installs.FindIndex((i) => i.program == prog.program);
            // Add from installed programs if applicable
            if (progInd != -1)
            {
                Shell.Install(prog.program, prog.location);
                installs.RemoveAt(progInd);
                Grid.Add(prog.location, prog.program);
            }
            else if (Inventory.HasProgram(prog.program))
            {
                Inventory.RemoveProgram(prog.program);
                Shell.Install(prog.program, prog.location);
                Grid.Add(prog.location, prog.program);
            }
        }
        // Add any programs that weren't reinstalled back to the inventory
        foreach (var prog in installs)
        {
            Inventory.AddProgram(prog.program);
        }
        // Reinitialize the program button UI
        CustUI.InitializeProgramButtons();
        SetLoadedPresetIndex(index);
        CustUI.UpdateCompileButtonColor();
    }

    public void Clear()
    {
        SetLoadedPresetIndex(PresetManager.noLoadedPreset);
        EnterMainMenu();
    }

    public void EnterMainMenu()
    {
        loadMenu.SetActive(false);
        saveNewMenu.SetActive(false);
        ClearSaveNewMenu();
    }

    private void ClearSaveNewMenu()
    {
        newPresetNameMenu.SetActive(false);
        SetSaveAsNewPresetName(string.Empty);
        newPresetInput.text = string.Empty;
    }

    public void EnterSaveNewMenu()
    {
        saveNewMenu.SetActive(true);
        ClearSaveNewMenu();
    }
}
