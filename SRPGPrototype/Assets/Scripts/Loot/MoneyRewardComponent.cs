using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoneyRewardComponent : MonoBehaviour
{
    [SerializeField] private string rewardName;

    protected string RewardName => rewardName;
    protected abstract int GetAmount();

    public LootUI.MoneyData GenerateMoneyData()
    {
        return new LootUI.MoneyData(GetAmount(), RewardName);
    }
}
