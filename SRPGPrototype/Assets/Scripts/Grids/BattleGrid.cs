using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleGrid : Grid<Combatant>
{
    [SerializeField] private Vector2Int dimensions = new Vector2Int(8, 8);
    public override Vector2Int Dimensions => dimensions;

    // Start is called before the first frame update
    void Awake()
    {
        Initialize();
    }
}
