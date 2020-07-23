using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Encounter
{
    public List<UnitEntry> units = new List<UnitEntry>();
    public List<UnitEntry> reinforcements = new List<UnitEntry>();

    [System.Serializable]
    public struct UnitEntry
    {
        public Vector2 pos;
        public Unit enemy;
    }
}
