using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BetterBundle<T> : ScriptableObject, ISerializationCallbackReceiver where T : UnityEngine.Object, IHasKey
{
    public string[] assetPaths;
    [SerializeField] private List<string> keys = new List<string>();
    [SerializeField] private List<T> items = new List<T>();
    private readonly Dictionary<string, T> dict = new Dictionary<string, T>();

    public void Clear()
    {
        dict.Clear();
    }

    public void Add(T item)
    {
        if (string.IsNullOrEmpty(item.Key))
            return;
        dict.Add(item.Key, item);
    }

    public bool TryGet(string key, out T item)
    {
        return dict.TryGetValue(key, out item);
    }

    public void OnBeforeSerialize()
    {
        items.Clear();
        keys.Clear();
        foreach (var kvp in dict)
        {
            items.Add(kvp.Value);
            keys.Add(kvp.Key);
        }
    }

    public void OnAfterDeserialize()
    {
        dict.Clear();
        int count = System.Math.Min(items.Count, keys.Count);
        for (int i = 0; i < count; i++)
        {
            dict.Add(keys[i], items[i]);
        }
    }
}
