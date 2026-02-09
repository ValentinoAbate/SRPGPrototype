using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIComponentDoNothing : AIComponent
{
    public override IEnumerable<Action> Actions
    {
        get
        {
            yield break;
        }
    }


    public override IEnumerator DoTurn(BattleGrid grid, AIUnit self)
    {
        yield break;
    }

    public override void Initialize(AIUnit self)
    {
        return;
    }
}
