using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LootableUtils
{
    public static T InstantiateWithVariants<T>(this T prefab, Transform parent) where T : MonoBehaviour, ILootable 
    {
        var item = Object.Instantiate(prefab.gameObject, parent).GetComponent<T>();
        item.ApplyVariants();
        return item;
    }
}
