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
        string key = s.DisplayName;
        if(presets.ContainsKey(key))
            return presets[key];
        presets.Add(key, new Shell.Preset[3]);
        return presets[key];
    }

    public int GetLoadedPreset(Shell s)
    {
        string key = s.DisplayName;
        if (loadedPresets.ContainsKey(key))
            return loadedPresets[key];
        return noLoadedPreset;
    }

    public void SetLoadedPreset(Shell s, int index)
    {
        string key = s.DisplayName;
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
        else if(index != noLoadedPreset)
        {
            loadedPresets.Add(key, index);
        }
    }

    public void Clear()
    {
        presets.Clear();
        loadedPresets.Clear();
    }
}
