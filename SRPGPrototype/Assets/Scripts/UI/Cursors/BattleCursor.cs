using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCursor : Cursor<Combatant>
{
    public override Grid<Combatant> Grid => battleGrid;
    public BattleGrid battleGrid;
}
