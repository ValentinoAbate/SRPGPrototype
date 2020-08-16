using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleGrid : Grid<Unit>
{
    [SerializeField] private Vector2Int dimensions = new Vector2Int(8, 8);
    public override Vector2Int Dimensions => dimensions;

    public System.Action<Unit> OnAddUnit { get; set; }

    // Start is called before the first frame update
    void Awake()
    {
        Initialize();
    }

    public override bool Add(Vector2Int pos, Unit obj)
    {
        if(base.Add(pos, obj))
        {
            OnAddUnit?.Invoke(obj);
            return true;
        }
        return false;
    }
}
