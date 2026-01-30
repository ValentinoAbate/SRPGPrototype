using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProgramUpgrade : MonoBehaviour, IHasKey
{
    public ProgramTriggerCondition Condition => GetComponent<ProgramTriggerCondition>();

    public string Key => key;
    [SerializeField] private string key;
    public bool Hidden { get => hidden; }
    [SerializeField] private bool hidden = false;
    public string TriggerName { get => Hidden ? ProgramDescriptionUI.Hide(displayName) : displayName; }
    [SerializeField] protected string displayName = string.Empty;

    public IReadOnlyList<ProgramUpgrade> Upgrades { get; private set; }
    public ProgramEffect[] ProgramEffects { get; private set; }
    public ProgramModifier[] ModifierEffects { get; private set; }
    public string Description => description;
    [SerializeField] [TextArea(2, 4)] private string description = string.Empty;
    public string DisplayName { get => displayName; }

    public void Initialize(Program program)
    {
        // Upgrades shouldn't include self
        var upgrades = new List<ProgramUpgrade>();
        foreach (Transform t in transform)
        {
            upgrades.AddRange(t.GetComponents<ProgramUpgrade>());
        }
        Upgrades = upgrades;
        ProgramEffects = GetComponents<ProgramEffect>();
        ModifierEffects = GetComponents<ProgramModifier>();
        foreach(var effect in ProgramEffects)
        {
            effect.Initialize(program);
        }
    }

#if UNITY_EDITOR
    public bool GenerateKey()
    {
        string newKey = name.Replace("Upgrade", string.Empty);
        if (key == newKey)
            return false;
        key = newKey;
        if (string.IsNullOrEmpty(key))
        {
            key = "x";
            Debug.LogError(key);
        }
        return true;
    }
#endif
}
