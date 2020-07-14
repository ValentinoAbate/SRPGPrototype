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
    public Player player;
    public Inventory inventory;

    private void Awake()
    {
        if(main == null)
        {
            main = this;
            if(Application.isPlaying)
                DontDestroyOnLoad(transform);
        }
        else if (Application.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
