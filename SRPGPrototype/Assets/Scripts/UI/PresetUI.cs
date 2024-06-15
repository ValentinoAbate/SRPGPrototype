using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PresetUI : MonoBehaviour
{
    [Header("Preset UI")]
    public TextMeshProUGUI presetNameText;
    public Button savePresetButton;
    public TextMeshProUGUI[] preset1ButtonTexts;
    public TextMeshProUGUI[] preset2ButtonTexts;
    public TextMeshProUGUI[] preset3ButtonTexts;
    public Button[] loadButtons;
    public Button[] saveNewButtons;

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
        if (index == PresetManager.noLoadedPreset)
        {
            savePresetButton.interactable = false;
            CustUI.UninstallAll();
            return;
        }
        var installs = new List<Shell.InstalledProgram>(Shell.Programs);
        foreach (var install in installs)
        {
            var prog = install.program;
            Shell.Uninstall(prog, prog.Pos);
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
            // Add from uninstalled programs if applicable
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
    }
}
