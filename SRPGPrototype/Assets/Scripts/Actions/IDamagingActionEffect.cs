using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagingActionEffect
{
    public bool DealsDamage { get; }
    public bool UsesPower { get; }
    public int ActionMacroDamage(BattleGrid grid, Action action, SubAction sub, Unit user, Queue<int> indices);
    public int CalculateDamage(BattleGrid grid, Action action, SubAction sub, Unit user, Unit target, ActionEffect.PositionData targetData, bool simulation);
}
