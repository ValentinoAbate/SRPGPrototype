using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootData<T> where T : MonoBehaviour, ILootable
{
    public delegate List<T> GenerateLootFunction(Loot<T> loot, out string lootName, out int declineBonus);
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

    public void Add(IEnumerable<T> items, string name, int declineBonus)
    {
        draws.Add(new Data(items, name, declineBonus));
    }

    public void Add(Loot<T> loot, GenerateLootFunction generator)
    {
        var items = generator(loot, out var lootName, out int declineBonus);
        Add(items, lootName, declineBonus);
    }

    public void Instantiate(Transform parent)
    {
        var temp = new List<T>();
        foreach (var draw in Draws)
        {
            temp.Clear();
            temp.AddRange(draw);
            draw.Clear();
            foreach(var item in temp)
            {
                draw.Add(item.InstantiateWithVariants(parent));
            }
        }
    }

    public class Data : List<T>
    {
        public string Name { get; }

        public int DeclineBonus { get; }

        public Data(IEnumerable<T> items, string name, int declineBonus) : base(items)
        {
            Name = name;
            DeclineBonus = declineBonus;
        }
    }
}
