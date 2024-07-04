using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShopGenerator : ScriptableObject
{
    public ShopManager.ShopID ShopID => shopID;
    [SerializeField] private ShopManager.ShopID shopID;

    public abstract ShopManager.ShopData GenerateShopData(Transform objectContainer);
}
