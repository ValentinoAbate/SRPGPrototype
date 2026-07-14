using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionCondition : MonoBehaviour
{
    public abstract bool CanUse(BattleGrid grid, Unit user, Action action, out string failMessage);
}
