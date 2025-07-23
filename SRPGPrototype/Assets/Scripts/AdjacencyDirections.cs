using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum AdjacencyDirections
{
    None = 0,
    Horizontal = 1,
    Diagonal = 2,
    Both = Horizontal | Diagonal
}
