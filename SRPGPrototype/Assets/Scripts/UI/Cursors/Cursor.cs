using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grid;

public abstract class Cursor<T> : MonoBehaviour where T : GridObject
{
    public System.Action<Vector2Int> OnClick { get; set; }
    public System.Action OnCancel { get; set; }
    public System.Action<Vector2Int> OnHighlight { get; set; }
    public System.Action<Vector2Int> OnUnHighlight { get; set; }

    public Vector2Int HighlightedPosition => Grid.GetPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));

    public abstract Grid<T> Grid { get; }
    private Vector3 previousMousePos = Vector3.zero;
    private Vector2Int previousMouseGridPos = Vector2Int.zero;

    private void Awake()
    {
        previousMouseGridPos = Grid<T>.OutOfBounds;
    }

    public void NullAllActions()
    {
        OnCancel = null;
        OnClick = null;
        OnHighlight = null;
        OnUnHighlight = null;
    }

    public void CheckInput()
    {
        if (Input.mousePosition != previousMousePos)
        {
            var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var mouseGridPos = Grid.GetPos(mouseWorldPos);
            if (mouseGridPos != previousMouseGridPos)
            {
                if (previousMouseGridPos != Grid<T>.OutOfBounds)
                    OnUnHighlight?.Invoke(previousMouseGridPos);
                if (Grid.IsLegal(mouseGridPos))
                {
                    transform.position = Grid.GetSpace(mouseGridPos);
                    OnHighlight?.Invoke(mouseGridPos);
                    previousMouseGridPos = mouseGridPos;
                }
                else
                {
                    previousMouseGridPos = Grid<T>.OutOfBounds;
                }
            }
            previousMousePos = Input.mousePosition;
        }
        if (Input.GetMouseButtonDown(0))
        {
            var mouseGridPos = HighlightedPosition;
            if (Grid.IsLegal(mouseGridPos))
                OnClick?.Invoke(mouseGridPos);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (OnCancel != null)
            {
                OnCancel.Invoke();
            }
        }
    }

    private void Update()
    {
        CheckInput();
    }
}
