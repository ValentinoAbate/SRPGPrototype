using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridObject : MonoBehaviour
{
    [SerializeField] [HideInInspector]
    private Vector2Int pos;
    public Vector2Int Pos { get => pos; set => pos = value; }
}
