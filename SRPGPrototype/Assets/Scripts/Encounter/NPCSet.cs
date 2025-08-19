using RandomUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCSet", menuName = "Encounter Generation/NPC Set")]
public class NPCSet : ScriptableObject
{
    [SerializeField] private List<NPCEntry> npcEntries = new List<NPCEntry>();

    public void GetNPCs(ref WeightedSet<Unit> primaryNPCs, ref WeightedSet<Unit> secondaryNPCs)
    {
        foreach (var entry in npcEntries)
        {
            if (entry.validator != null && !entry.validator.IsValid(entry.unit))
                continue;
            WeightedSet<Unit> set;
            if (entry.isPrimary)
            {
                set = primaryNPCs ??= new WeightedSet<Unit>();
            }
            else
            {
                set = secondaryNPCs ??= new WeightedSet<Unit>();
            }
            set.Add(entry.unit, entry.weight);
        }
    }

    [System.Serializable]
    public class NPCEntry
    {
        public Unit unit;
        public UnitValidator validator;
        public float weight = 1;
        public bool isPrimary;
    }
}
