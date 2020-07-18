using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCursor : MonoBehaviour
{
    public System.Action<Vector2Int> OnClick { get; set; }
    public System.Action OnCancel { get; set; }
    public System.Action<Vector2Int> OnHighlight { get; set; }
    public System.Action<Vector2Int> OnUnHighlight { get; set; }

    public BattleGrid grid;
    public BattleUI ui;
    private Vector3 previousMousePos = Vector3.zero;
    private Vector2Int previousMouseGridPos = Vector2Int.zero;

    private void Awake()
    {
        previousMouseGridPos = Grid<Combatant>.OutOfBounds;
    }

    private void Update()
    {
        if(Input.mousePosition != previousMousePos)
        {
            var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var mouseGridPos = grid.GetPos(mouseWorldPos);
            if(mouseGridPos != previousMouseGridPos)
            {
                if (previousMouseGridPos != Grid<Combatant>.OutOfBounds)
                    OnUnHighlight?.Invoke(previousMouseGridPos);
                if (grid.IsLegal(mouseGridPos))
                {
                    transform.position = grid.GetSpace(mouseGridPos);
                    OnHighlight?.Invoke(mouseGridPos);
                    previousMouseGridPos = mouseGridPos;
                }
                else
                {
                    previousMouseGridPos = Grid<Combatant>.OutOfBounds;
                }
            }
            previousMousePos = Input.mousePosition;
        }
        if(Input.GetMouseButtonDown(0))
        {
            var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var mouseGridPos = grid.GetPos(mouseWorldPos);
            if(grid.IsLegal(mouseGridPos))
                OnClick?.Invoke(mouseGridPos);
        }
        else if(Input.GetMouseButtonDown(1))
        {
            if(OnCancel != null)
            {
                OnCancel.Invoke();
            }
        }
    }
}
