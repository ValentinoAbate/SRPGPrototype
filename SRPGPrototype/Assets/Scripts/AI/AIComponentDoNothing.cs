using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIComponentDoNothing : AIComponent<AIUnit>
{
    public override List<Action> Actions => new List<Action>();

    public override IEnumerator DoTurn(BattleGrid grid, AIUnit self)
    {
        yield break;
    }

    public override void Initialize(AIUnit self)
    {
        return;
    }
}
