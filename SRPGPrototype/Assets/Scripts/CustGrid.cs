﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustGrid : Grid<Program>
{
    public Shell shell;
    public override Vector2Int Dimensions => shell.custArea.Dimensions;

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        var offsetsSet = shell.custArea.OffsetsSet;
        for (int x = 0; x < Dimensions.x; ++x)
        {
            for (int y = 0; y < Dimensions.y; ++y)
            {
                var pos = new Vector2Int(x, y);
                if (!offsetsSet.Contains(pos))
                    Gizmos.DrawIcon(GetSpace(pos),"CollabError", true);
            }
        }
    }

    private void Awake()
    {
        Initialize();
        foreach(var prog in shell.programs)
            Add(prog.location, prog.program);
    }

    public override void Add(Vector2Int pos, Program obj)
    {
        foreach(var shiftPos in obj.shape.OffsetsShifted(pos))
        {
            Set(shiftPos, obj);
        }
        obj.Pos = pos;
        obj.Show(pos, this);
    }

    public override void Remove(Program obj)
    {
        foreach (var shiftPos in obj.shape.OffsetsShifted(obj.Pos))
        {
            Set(shiftPos, null);
        }
        obj.Pos = OutOfBounds;
        obj.Hide(this);
    }

}
