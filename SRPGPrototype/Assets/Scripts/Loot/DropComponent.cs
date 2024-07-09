using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DropComponent<T> : MonoBehaviour where T : ILootable
{
    protected const int defaultRandomDeclineRange = 3;
    protected const int nullDeclineBonusOverride = int.MinValue;
    [SerializeField] private string dropName;
    [SerializeField] protected int declineBonusOverride = nullDeclineBonusOverride;
    public List<T> GenerateDrop(Loot<T> loot, out string name, out int declineBonus)
    {
        name = dropName;
        var drop = GenerateDrop(loot);
        declineBonus = GenerateDeclineBonus(drop);
        return drop;
    }
    protected abstract List<T> GenerateDrop(Loot<T> loot);
    protected virtual int GenerateDeclineBonus(List<T> drops)
    {
        if (declineBonusOverride != nullDeclineBonusOverride)
            return declineBonusOverride;
        int bonus = 0;
        foreach(var drop in drops)
        {
            bonus += drop.Rarity switch
            {
                Rarity.Common => 3,
                Rarity.Uncommon => 5,
                Rarity.Rare => 7,
                Rarity.Elite => 10,
                _ => 0,
            };
        }
        if(bonus > 0)
        {
            bonus += RandomUtils.RandomU.instance.RandomInt(System.Math.Max(-defaultRandomDeclineRange, -bonus), defaultRandomDeclineRange + 1);
        }
        return bonus;
    }
}
