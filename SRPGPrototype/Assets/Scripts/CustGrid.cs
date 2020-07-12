using System.Collections;
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
        // Add preinstalled programs
        foreach(var preInstall in shell.programs)
        {
            Add(preInstall.location, preInstall.program);
            // Add the "fixed" program attribute
            preInstall.program.attributes |= Program.Attributes.Fixed;
        }


        // Spawn blocked tile UI in blocked tiles
        var offsetsSet = shell.custArea.OffsetsSet;
        for (int x = 0; x < Dimensions.x; ++x)
        {
            for (int y = 0; y < Dimensions.y; ++y)
            {
                var pos = new Vector2Int(x, y);
                if (!offsetsSet.Contains(pos))
                    SpawnTileUI(pos, TileUI.Type.CustBlocked);
            }
        }
    }

    public override bool Add(Vector2Int addPos, Program obj)
    {
        var positions = obj.shape.OffsetsShifted(addPos);
        foreach(var pos in positions)
        {
            if(!IsLegal(pos))
            {
                Debug.LogWarning("Program: " + obj.name + " attempted to be added to an illegal pos:  " + pos.ToString());
                return false;
            }
            if (!IsEmpty(pos))
            {
                Debug.LogWarning("Program: " + obj.name + " overlaps another program at " + pos.ToString());
                return false;
            }
        }
        foreach (var pos in positions)
        {
            Set(pos, obj);
        }
        obj.Pos = addPos;
        obj.Show(addPos, this);
        return true;
    }

    public override void Remove(Program obj)
    {
        foreach (var pos in obj.shape.OffsetsShifted(obj.Pos))
        {
            Set(pos, null);
        }
        obj.Hide(this);
        obj.Pos = OutOfBounds;
    }

}
