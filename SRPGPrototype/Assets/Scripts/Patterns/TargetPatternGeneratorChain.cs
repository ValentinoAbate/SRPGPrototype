using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPatternGeneratorChain : TargetPatternGenerator
{
    public enum Adjacency
    { 
        Horizontal,
        Diagonal,
        Both,
    }

    public Adjacency adjacency = Adjacency.Diagonal;

    public override IEnumerable<Vector2Int> Generate(BattleGrid grid, Unit user, Vector2Int targetPos)
    {
        // Doesn't work on empty tiles
        if (grid.IsEmpty(targetPos))
            return new Vector2Int[0];
        var targets = new HashSet<Vector2Int>();
        // Get targets
        if(adjacency == Adjacency.Horizontal)
        {
            GetChainTargets(targetPos, grid, Vector2IntExtensions.Adjacent, ref targets);
        }
        else if(adjacency == Adjacency.Diagonal)
        {
            GetChainTargets(targetPos, grid, Vector2IntExtensions.AdjacentDiagonal, ref targets);
        }
        else if(adjacency == Adjacency.Both)
        {
            GetChainTargets(targetPos, grid, Vector2IntExtensions.AdjacentBoth, ref targets);
        }
        // Return targets
        return targets;
    }

    /// <summary>
    /// Recursive function that continues the chain on valid adjacent targets
    /// </summary>
    public void GetChainTargets(Vector2Int position, BattleGrid grid, System.Func<Vector2Int, IEnumerable<Vector2Int>> adjFn, ref HashSet<Vector2Int> positions)
    {
        positions.Add(position);
        foreach (var adj in adjFn(position))
        {
            if (grid.IsLegal(adj) && !grid.IsEmpty(adj) && !positions.Contains(adj))
            {
                GetChainTargets(adj, grid, adjFn, ref positions);
            }
        }
    }
}
