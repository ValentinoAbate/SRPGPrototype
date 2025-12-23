using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pattern
{
    public ISet<Vector2Int> OffsetsSet => new HashSet<Vector2Int>(Offsets);
    public IReadOnlyList<Vector2Int> Offsets => patternOffsets;
    [SerializeField]
    private List<Vector2Int> patternOffsets = new List<Vector2Int>();
    public Vector2Int Dimensions
    {
        get => dimensions;
        set => dimensions = value;
    }
    [SerializeField]
    private Vector2Int dimensions = new Vector2Int(1, 1);

    public Vector2Int Center => new Vector2Int(dimensions.x / 2, dimensions.y / 2);

    public IEnumerable<Vector2Int> OffsetsShifted(Vector2Int shift, bool center = true)
    {
        var modShift = center ? shift - Center : shift;
        foreach(var offset in Offsets)
        {
            yield return offset + modShift;
        }
    }

    public void AddOffset(Vector2Int offset)
    {
        patternOffsets.Add(offset);
    }

    public void AddOffsets(IEnumerable<Vector2Int> offsets)
    {
        patternOffsets.AddRange(offsets);
    }
}
