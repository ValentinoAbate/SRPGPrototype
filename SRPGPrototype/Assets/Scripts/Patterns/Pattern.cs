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

    public string Save()
    {
        var builder = new System.Text.StringBuilder();
        builder.Append(dimensions.Save());
        builder.Append(SaveManager.separator);
        if(patternOffsets.Count > 0)
        {
            for (int i = 0; i < patternOffsets.Count - 1; i++)
            {
                Vector2Int offset = patternOffsets[i];
                builder.Append(offset.Save());
                builder.Append(SaveManager.separator);
            }
            builder.Append(patternOffsets[patternOffsets.Count - 1].Save());
        }
        return builder.ToString();
    }

    public void Load(string data)
    {
        var args = data.Split(SaveManager.separator);
        patternOffsets.Clear();
        if(args.Length <= 0)
        {
            dimensions = Vector2Int.zero;
            return;
        }
        dimensions = Vector2IntUtils.FromString(args[0]);
        for (int i = 1; i < args.Length; i++)
        {
            patternOffsets.Add(Vector2IntUtils.FromString(args[i]));
        }
    }
}
