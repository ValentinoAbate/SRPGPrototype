using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootData<T> where T : ILootable
{
    public delegate List<T> GenerateLootFunction(Loot<T> loot, out string lootName);
    public IReadOnlyList<Data> Draws => draws;
    private readonly List<Data> draws = new List<Data>();
    public LootData(params Data[] args)
    {
        draws.EnsureCapacity(draws.Count);
        draws.AddRange(args);
    }

    public LootData(int capacity)
    {
        draws.Capacity = capacity;
    }

    public void Add(IEnumerable<T> items, string name)
    {
        draws.Add(new Data(items, name));
    }

    public void Add(Loot<T> loot, GenerateLootFunction generator)
    {
        var items = generator(loot, out var lootName);
        Add(items, lootName);
    }

    public class Data : List<T>
    {
        public string Name { get; }

        public Data(IEnumerable<T> items, string name) : base(items)
        {
            Name = name;
        }
    }
}
