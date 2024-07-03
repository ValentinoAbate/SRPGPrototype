using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopkeeperUnit : AIUnit
{
    [SerializeField] private Action[] shopActions;
    public override AIComponent<AIUnit> AI => ai;
    private AIComponent<AIUnit> ai;

    protected override void Initialize()
    {
        ai = GetComponent<AIComponent<AIUnit>>();
        base.Initialize();
        for (int i = 0; i < shopActions.Length; i++)
        {
            shopActions[i] = shopActions[i].Validate(ActionTransform);
        }
    }

    public override IReadOnlyCollection<Action> GetContextualActions(Unit user, BattleGrid grid)
    {
        return grid.IsAdjacent(this, user) ? shopActions : System.Array.Empty<Action>();
    }
}
