using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grid;

public class BattleCursor : Cursor<Unit>
{
    public override Grid<Unit> Grid => battleGrid;
    public BattleGrid battleGrid;
}
