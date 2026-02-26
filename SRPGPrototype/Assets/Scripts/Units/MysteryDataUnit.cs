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
        Money,
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

    protected override void Initialize()
    {
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
        base.Initialize();
    }
}
