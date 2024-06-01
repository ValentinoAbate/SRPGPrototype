using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is a singleton that shouldn't be destroyed
/// </summary>
[ExecuteAlways]
public class PersistantData : MonoBehaviour
{
    public static PersistantData main;
    public Inventory inventory;
    public MapManager mapManager;
    public PresetManager presetManager;

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
        mapManager.Clear();
        inventory.Initialize();
        mapManager.Generate();
    }
}
