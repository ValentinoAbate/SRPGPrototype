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
        shopManager.Initialize();
        CurrentId = 0;
    }

    public void SaveRunData(SaveManager.RunData runData)
    {
        runData.currId = CurrentId;
        runData.inv = inventory.Save();
    }

    public void LoadRunData(SaveManager.RunData data, SaveManager.Loader loader)
    {
        ResetRunData();
        inventory.Load(data.inv, loader);
        CurrentId = data.currId;
    }
}
