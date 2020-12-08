using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProgramUpgrade : ProgramTrigger
{
    public ProgramTrigger[] Upgrades { get; private set; }
    public ProgramEffect[] ProgramEffects { get; private set; }
    public string Description => description;
    [SerializeField] [TextArea(2, 4)] private string description = string.Empty;
    public string DisplayName { get => displayName; }
    private void Awake()
    {
        // Upgrades shouldn't include self
        Upgrades = GetComponentsInChildren<ProgramTrigger>(true).Where((t) => t != this).ToArray();
        ProgramEffects = GetComponents<ProgramEffect>();
    }
}
