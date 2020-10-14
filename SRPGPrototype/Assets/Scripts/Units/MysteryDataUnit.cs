using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryDataUnit : AIUnit
{
    public enum Category
    {
        Standard,
        Color,
        Ability,
        Capacity,
        Shell,
        Gamble,
        Boss,
        Unique,
    }

    public enum Quality
    {
        None = -1,
        Common,
        Uncommon,
        Rare,
    }

    public Category LootCategory { get => category; }
    [SerializeField] private Category category = Category.Standard;
    public Quality LootQuality { get => quality; }
    [SerializeField] private Quality quality = Quality.None;

    [SerializeField] private SpriteRenderer frameSprite = null;
    [SerializeField] private Color commonColor = Color.white;
    [SerializeField] private Color uncommonColor = Color.white;
    [SerializeField] private Color rareColor = Color.white;
    [SerializeField] private Color gambleColor = Color.white;

    public override AIComponent<AIUnit> AI => ai;

    private AIComponent<AIUnit> ai;

    private void Awake()
    {
        ai = GetComponent<AIComponent<AIUnit>>();
        switch (LootQuality)
        {
            case Quality.Common:
                frameSprite.color = commonColor;
                break;
            case Quality.Uncommon:
                frameSprite.color = uncommonColor;
                break;
            case Quality.Rare:
                frameSprite.color = rareColor;
                break;
        }
    }

    public override IEnumerator OnBattleEnd(EncounterManager manager)
    {
        if(Dead)
            yield break;
        var progDrops = GetComponentsInChildren<DropComponent<Program>>();
        foreach (var drop in progDrops)
            manager.GenerateProgramLoot += drop.GenerateDrop;
        var shellDrops = GetComponentsInChildren<DropComponent<Shell>>();
        foreach (var drop in shellDrops)
            manager.GenerateShellLoot += drop.GenerateDrop;
    }
}
