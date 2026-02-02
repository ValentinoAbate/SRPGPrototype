using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is a singleton that shouldn't be destroyed
/// </summary>
//[ExecuteAlways]
public class PersistantData : MonoBehaviour
{
    public static PersistantData main;
    public Inventory inventory;
    public MapManager mapManager;
    public PresetManager presetManager;
    public ShopManager shopManager;
    public LootManager loot;
    public ProgramFuser programFuser;

    public int CurrentId { get; private set; } = 0;
    public int NewId => ++CurrentId;

    private void Awake()
    {
        if(main == null)
        {
            main = this;
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(transform);
                ResetRunData();
            }
        }
        else if (Application.isPlaying)
        {
            Destroy(gameObject);
        }
    }

    public void ResetRunData()
    {
        presetManager.Clear();
        inventory.Clear();
        CurrentId = 0;
        shopManager.Initialize();
    }

    public void SaveRunData(SaveManager.RunData runData)
    {
        runData.currId = CurrentId;
        runData.inv = inventory.Save(ref runData.fArgs);
        runData.map = mapManager.Save();
        runData.shops = shopManager.Save(ref runData.fArgs);
        runData.presets = presetManager.Save();
    }

    public void LoadRunData(SaveManager.RunData data, SaveManager.Loader loader)
    {
        inventory.Load(data.inv, loader);
        mapManager.Load(data.map);
        shopManager.Load(data.shops, loader);
        presetManager.Load(data.presets, loader);
        CurrentId = data.currId;
    }
}
