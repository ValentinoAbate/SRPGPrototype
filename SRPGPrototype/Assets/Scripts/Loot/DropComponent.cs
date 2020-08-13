using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DropComponent<T> : MonoBehaviour where T : ILootable
{
    public abstract List<T> GenerateDrop(LootManager manager);
}
