using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootData<T> : List<List<T>> where T : ILootable
{
    public LootData(params List<T>[] draws) : base(draws) { }

    public LootData(int capacity) : base(capacity) { }
}
