using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIComponentDoNothing : AIComponent<UnitEnemy>
{
    public override IEnumerator DoTurn(BattleGrid grid, UnitEnemy self)
    {
        yield break;
    }
}
