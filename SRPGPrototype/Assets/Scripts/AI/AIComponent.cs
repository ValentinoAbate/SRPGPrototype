using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIComponent<T> : MonoBehaviour where T : Combatant
{
    // amount of time to pause for each square moved
    public const float moveDelay = 0.1f;

    public virtual bool StandardActivateChargedAction => true;

    public abstract IEnumerator DoTurn(BattleGrid grid, T self);

    protected IEnumerator MoveAlongPath(BattleGrid grid, T self, List<Vector2Int> path, int maxDistance = int.MaxValue)
    {
        // Move along the path until within range
        for (int i = 0; i < maxDistance && i < path.Count; ++i)
        {
            //yield return new WaitWhile(() => self.PauseHandle.Paused);
            grid.MoveAndSetWorldPos(self, path[i]);
            yield return new WaitForSeconds(moveDelay);
        }
    }

    public int CompareTargetPriority(Combatant obj1, int pathDist1, Combatant obj2, int pathDist2)
    {
        // First compare grid distance
        int distCmp = pathDist1.CompareTo(pathDist2);
        if (distCmp != 0)
            return distCmp;
        // If grid distance is the same, compare hp
        if (obj1.HP != obj2.HP)
            return obj1.HP.CompareTo(obj2.HP);
        // Later will compare based on explicit sorting order (names)
        return 0;
    }
}
