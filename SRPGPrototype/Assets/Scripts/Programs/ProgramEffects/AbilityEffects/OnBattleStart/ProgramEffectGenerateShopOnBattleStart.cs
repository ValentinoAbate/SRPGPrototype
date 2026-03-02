using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectGenerateShopOnBattleStart : ProgramEffectAddOnBattleStartAbility
{
    public override string AbilityName => string.Empty;

    [SerializeField] private ShopGenerator shopGenerator;

    public override void Ability(BattleGrid grid, Unit unit)
    {
        PersistantData.main.shopManager.UpdateShop(shopGenerator);
    }
}
