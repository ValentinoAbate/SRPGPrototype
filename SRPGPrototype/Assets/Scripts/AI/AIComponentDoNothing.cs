using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIComponentDoNothing : AIComponent<Unit>
{
    public override List<Action> Actions => new List<Action>();

    public override IEnumerator DoTurn(BattleGrid grid, Unit self)
    {
        yield break;
    }
}
