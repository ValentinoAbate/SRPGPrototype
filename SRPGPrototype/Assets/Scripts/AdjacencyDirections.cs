using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum AdjacencyDirections
{
    None = 0,
    HorizontalVertical = 1,
    Diagonal = 2,
    Both = HorizontalVertical | Diagonal
}
