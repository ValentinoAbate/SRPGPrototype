using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEffectOpenShop : ActionEffect
{
    [SerializeField] private ShopManager.ShopID shopID;
    public override void ApplyEffect(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, PositionData targetData)
    {
        PersistantData.main.shopManager.ShowShop(shopID);
    }
}
