using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyRewardComponentRange : MoneyRewardComponent
{
    [SerializeField] private int baseAmount;
    [SerializeField] private int variance;
    protected override int GetAmount()
    {
        return baseAmount + RandomUtils.RandomU.instance.RandomInt(-variance, variance + 1);
    }
}
