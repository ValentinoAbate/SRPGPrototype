using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DropComponent<T> : MonoBehaviour where T : ILootable
{
    [SerializeField] private string dropName;
    public List<T> GenerateDrop(Loot<T> loot, out string name)
    {
        name = dropName;
        return GenerateDrop(loot);
    }
    protected abstract List<T> GenerateDrop(Loot<T> loot);
}
