using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PatternUtils
{
    public static IReadOnlyList<Pattern> GetAllOverlaps(Pattern pattern1, Pattern pattern2)
    {
        var overlaps = new List<Pattern>();
        var maxDimensions = new Vector2Int(pattern1.Dimensions.x + pattern2.Dimensions.x - 1, pattern1.Dimensions.y + pattern2.Dimensions.y - 1);
        var visitedOffsets = new HashSet<Vector2Int>();
        var overlapSet = new HashSet<Vector2Int>();
        foreach(var p1 in ValidPositionsWithinGrid(pattern1.Dimensions, maxDimensions))
        {
            foreach(var p2 in ValidPositionsWithinGrid(pattern2.Dimensions, maxDimensions))
            {
                var offset = p1 - p2;
                if (visitedOffsets.Contains(offset))
                    continue;
                visitedOffsets.Add(offset);
                // Overlap patterns
                overlapSet.Clear();
                foreach(var pos in pattern1.OffsetsShifted(p1, false))
                {
                    overlapSet.Add(pos);
                }
                foreach (var pos in pattern2.OffsetsShifted(p2, false))
                {
                    if (overlapSet.Contains(pos))
                        continue;
                    overlapSet.Add(pos);
                }
                // 0 overlap, continue
                if (overlapSet.Count == (pattern1.Offsets.Count + pattern2.Offsets.Count))
                    continue;
                // Create actual ovelap pattern
                var overlap = new Pattern();
                overlap.AddOffsets(overlapSet);
                int xDim = System.Math.Max(p1.x + pattern1.Dimensions.x, p2.x + pattern2.Dimensions.x);
                int yDim = System.Math.Max(p1.y + pattern1.Dimensions.y, p2.y + pattern2.Dimensions.y);
                overlap.Dimensions = new Vector2Int(xDim, yDim);
                if (HasDuplicate(overlaps, overlap, overlapSet))
                    continue;
                overlaps.Add(overlap);
            }
        }
        return overlaps;
    }

    private static bool HasDuplicate(IEnumerable<Pattern> patterns, Pattern newPattern, ISet<Vector2Int> newPatternSet)
    {
        foreach (var pattern in patterns)
        {
            if (pattern.Offsets.Count != newPattern.Offsets.Count)
                continue;
            if (newPattern.Dimensions != pattern.Dimensions)
                continue;
            bool isDuplicate = true;
            foreach (var otherOffset in pattern.Offsets)
            {
                if (!newPatternSet.Contains(otherOffset))
                {
                    isDuplicate = false;
                    break;
                }
            }
            if (isDuplicate)
            {
                return true;
            }
        }
        return false;
    }

    private static IEnumerable<Vector2Int> ValidPositionsWithinGrid(Vector2Int patternDimesions, Vector2Int gridDimensions)
    {
        for (int x = 0; x < gridDimensions.x; ++x)
        {
            if (x + patternDimesions.x > gridDimensions.x)
                continue;
            for (int y = 0; y < gridDimensions.y; ++y)
            {
                if (y + patternDimesions.y > gridDimensions.y)
                    continue;
                yield return new Vector2Int(x, y);
            }
        }
    }
}
