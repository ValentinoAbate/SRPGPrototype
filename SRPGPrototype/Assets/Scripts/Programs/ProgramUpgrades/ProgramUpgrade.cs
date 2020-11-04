using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramUpgrade : MonoBehaviour
{
    public ProgramUpgrade[] Upgrades => GetComponentsInChildren<ProgramUpgrade>();
    public ProgramEffect[] ProgramEffects { get; private set; }
    public string DisplayName => displayName;
    [SerializeField] private string displayName = string.Empty;
    public string Description => description;
    [SerializeField] [TextArea(2, 4)] private string description = string.Empty;

    private void Awake()
    {
        ProgramEffects = GetComponents<ProgramEffect>();
    }
}
