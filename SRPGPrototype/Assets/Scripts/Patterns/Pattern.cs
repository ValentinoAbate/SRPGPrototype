﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Pattern
{
    private static readonly Vector2Int autoCenter = new Vector2Int(-1, 1);
    public ISet<Vector2Int> OffsetsSet => new HashSet<Vector2Int>(Offsets);
    public IEnumerable<Vector2Int> Offsets => patternOffsets;
    [SerializeField]
    private List<Vector2Int> patternOffsets = new List<Vector2Int>();
    public Vector2Int Dimensions => dimensions;
    [SerializeField]
    private Vector2Int dimensions = new Vector2Int(1, 1);

    public Vector2Int Center => new Vector2Int(dimensions.x / 2, dimensions.y / 2);

    public IEnumerable<Vector2Int> OffsetsShifted(Vector2Int shift, bool center = true)
    {
        var modShift = center ? shift - Center : shift;
        return Offsets.Select((o) => o + modShift);
    }

    public void AddOffset(Vector2Int offset)
    {
        patternOffsets.Add(offset);
    }
}
