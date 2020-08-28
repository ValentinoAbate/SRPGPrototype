using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramEffectCustRestrictionAdjacentColors : ProgramEffectCustRestriction
{
    protected override bool Restriction(Shell shell, out string errorMessage)
    {
        errorMessage = noErrorMessage;
        var map = shell.InstallMap;
        for(int x = 0; x < map.GetLength(0); ++x)
        {
            for(int y = 0; y < map.GetLength(1); ++y)
            {
                var program = map[x, y];
                if (program == null)
                    continue;
                var pos = new Vector2Int(x, y);
                var adjacent = new Vector2Int[]
                {
                    pos + Vector2Int.up,    
                    pos + Vector2Int.down,    
                    pos + Vector2Int.left,    
                    pos + Vector2Int.right,    
                };
                foreach(var adj in adjacent)
                {
                    if (adj.x < 0 || adj.x >= map.GetLength(0) || adj.y < 0 || adj.y >= map.GetLength(1))
                        continue;
                    var adjProgram = map[adj.x, adj.y];
                    if (adjProgram == null)
                        continue;
                    if (adjProgram != program && adjProgram.color == program.color)
                    {
                        errorMessage = "Compille Error: Programs " + program.DisplayName + " and " + adjProgram.DisplayName 
                            + " are adjacent and are both " + program.color.ToString();
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
