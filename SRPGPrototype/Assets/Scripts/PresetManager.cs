using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresetManager : MonoBehaviour
{
    public const int numPresets = 3;
    public const int noLoadedPreset = -1;
    private readonly Dictionary<string, Shell.Preset[]> presets = new Dictionary<string, Shell.Preset[]>();
    private readonly Dictionary<string, int> loadedPresets = new Dictionary<string, int>();

    public Shell.Preset[] GetPresets(Shell s)
    {
        return GetPresets(s.Key);
    }

    private Shell.Preset[] GetPresets(string key)
    {
        if (presets.ContainsKey(key))
            return presets[key];
        presets.Add(key, new Shell.Preset[numPresets]);
        return presets[key];
    }

    public int GetLoadedPreset(Shell s)
    {
        string key = s.Key;
        if (loadedPresets.ContainsKey(key))
            return loadedPresets[key];
        return noLoadedPreset;
    }

    public void SetLoadedPreset(Shell s, int index)
    {
        SetLoadedPreset(s.Key, index);
    }

    private void SetLoadedPreset(string key, int index)
    {
        if (loadedPresets.ContainsKey(key))
        {
            if (index == noLoadedPreset)
            {
                loadedPresets.Remove(key);
            }
            else
            {
                loadedPresets[key] = index;
            }
        }
        else if (index != noLoadedPreset)
        {
            loadedPresets.Add(key, index);
        }
    }

    public void Clear()
    {
        presets.Clear();
        loadedPresets.Clear();
    }

    public List<SaveManager.PresetData> Save()
    {
        var data = new List<SaveManager.PresetData>(presets.Count * numPresets);
        foreach(var kvp in presets)
        {
            for (int i = 0; i < kvp.Value.Length; i++)
            {
                var preset = kvp.Value[i];
                if (preset == null || preset.Programs.Count <= 0)
                    continue;
                var savedPreset = new SaveManager.PresetData()
                {
                    k = kvp.Key,
                    name = preset.DisplayName,
                    lv = preset.Level,
                    load = loadedPresets.TryGetValue(kvp.Key, out int ind) && ind == i,
                    ind = i,
                    prs = new List<SaveManager.InstalledProgramId>(preset.Programs.Count),
                };
                foreach(var prog in preset.Programs)
                {
                    savedPreset.prs.Add(new SaveManager.InstalledProgramId()
                    {
                        id = prog.program.Id,
                        p = prog.location,
                    });
                }
                data.Add(savedPreset);
            }
        }
        return data;
    }

    public void Load(List<SaveManager.PresetData> presetData, SaveManager.Loader loader)
    {
        Clear();
        foreach(var savedPreset in presetData)
        {
            var preset = new Shell.Preset()
            {
                DisplayName = savedPreset.name,
                Level = savedPreset.lv,
            };
            // Load programs
            foreach(var savedProgram in savedPreset.prs)
            {
                if (loader.LoadedPrograms.TryGetValue(savedProgram.id, out var program))
                {
                    preset.Programs.Add(new Shell.InstalledProgram(program, savedProgram.p));
                }
            }
            // Setup preset
            var presets = GetPresets(savedPreset.k);
            presets[savedPreset.ind] = preset;
            if (savedPreset.load)
            {
                SetLoadedPreset(savedPreset.k, savedPreset.ind);
            }
        }
    }
}
