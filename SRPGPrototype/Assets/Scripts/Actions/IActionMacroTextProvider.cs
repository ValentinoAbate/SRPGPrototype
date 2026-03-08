using System.Collections;
using System.Collections.Generic;

public interface IActionMacroTextProvider
{
    public string GetText(Queue<int> indices, Unit unit, BattleGrid grid);
}
