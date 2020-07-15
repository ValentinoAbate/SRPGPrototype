using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIComponentDoNothing : AIComponent<EnemyUnit>
{
    public override List<Action> Actions => new List<Action>();

    public override IEnumerator DoTurn(BattleGrid grid, EnemyUnit self)
    {
        yield break;
    }
}
