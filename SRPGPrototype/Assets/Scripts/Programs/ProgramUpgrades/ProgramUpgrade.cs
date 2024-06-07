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

    public override void Initialize(Program program)
    {
        // Upgrades shouldn't include self
        var trigList = new List<ProgramTrigger>();
        foreach (Transform t in transform)
        {
            trigList.AddRange(t.GetComponents<ProgramTrigger>());
        }
        Upgrades = trigList.ToArray();
        ProgramEffects = GetComponents<ProgramEffect>();
        foreach(var effect in ProgramEffects)
        {
            effect.Initialize(program);
        }
    }
}
