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

    public static LootData<Program>.Data GenerateLootData(this DropComponent<Program> drop)
    {
        return new LootData<Program>.Data(drop.GenerateDrop(PersistantData.main.loot.ProgramLoot, out var name, out int declineBonus), name, declineBonus);
    }


    public static LootData<Shell>.Data GenerateLootData(this DropComponent<Shell> drop)
    {
        return new LootData<Shell>.Data(drop.GenerateDrop(PersistantData.main.loot.ShellLoot, out var name, out int declineBonus), name, declineBonus);
    }
}
