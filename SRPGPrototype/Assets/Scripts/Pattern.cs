using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Pattern
{
    public ISet<Vector2Int> OffsetsSet => new HashSet<Vector2Int>(patternOffsets);
    public IEnumerable<Vector2Int> Offsets => patternOffsets;
    [SerializeField]
    private List<Vector2Int> patternOffsets = new List<Vector2Int>();
    public Vector2Int Dimensions => dimensions;
    [SerializeField]
    private Vector2Int dimensions = new Vector2Int(1, 1);

    public IEnumerable<Vector2Int> OffsetsShifted(Vector2Int shift)
    {
        return patternOffsets.Select((o) => o + shift);
    }
}
