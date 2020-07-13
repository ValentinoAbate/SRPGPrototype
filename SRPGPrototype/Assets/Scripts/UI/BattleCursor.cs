using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCursor : MonoBehaviour
{
    public System.Action<Vector2Int> OnClick { get; set; } = null;
    public System.Action OnCancel { get; set; } = null;
    public BattleGrid grid;
    private Vector3 previousMousePos = Vector3.zero;
    private void Update()
    {
        if(Input.mousePosition != previousMousePos)
        {
            var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var mouseGridPos = grid.GetPos(mouseWorldPos);
            if(grid.IsLegal(mouseGridPos))
            {
                transform.position = grid.GetSpace(mouseGridPos);
            }
        }
        if(Input.GetMouseButtonDown(0) && OnClick != null)
        {
            var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var mouseGridPos = grid.GetPos(mouseWorldPos);
            if (grid.IsLegal(mouseGridPos))
            {
                OnClick.Invoke(mouseGridPos);
            }
        }
        else if(Input.GetMouseButtonDown(1) && OnCancel != null)
        {
            OnCancel.Invoke();
        }
    }
}

