using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustGrid : Grid.Grid<Program>
{
    public Shell Shell
    {
        get => shell;
        set
        {
            if (value == shell)
                return;
            // Clear previous shell if applicable
            if(shell != null)
            {
                foreach (var program in this)
                {
                    program.Hide(this);
                }
                blockedUI.ForEach((e) => RemoveTileUI(e));
                blockedUI.Clear();
            }
            // Don't try to reinitialize if null
            if (value == null)
                return;
            // Set the shell value
            shell = value;
            // Reinitialize the grid
            Initialize();

            // Add already installed programs
            foreach (var program in shell.Programs)
            {
                Add(program.location, program.program);
            }

            // Spawn blocked tile UI in blocked tiles
            var offsetsSet = shell.CustArea.OffsetsSet;
            for (int x = 0; x < Dimensions.x; ++x)
            {
                for (int y = 0; y < Dimensions.y; ++y)
                {
                    var pos = new Vector2Int(x, y);
                    if (!offsetsSet.Contains(pos))
                    {
                        blockedUI.Add(SpawnTileUI(pos, TileUI.Type.CustBlocked));
                    }
                }
            }
        }
    }

    private List<TileUI.Entry> blockedUI = new List<TileUI.Entry>();
    private Shell shell = null;
    public override Vector2Int Dimensions => Shell.CustArea.Dimensions;

    protected override void OnDrawGizmos()
    {
        if (PersistantData.main == null)
            return;

        //base.OnDrawGizmos();
        //shell = PersistantData.main.inventory.EquippedShell;
        //var offsetsSet = shell.CustArea.OffsetsSet;
        //for (int x = 0; x < Dimensions.x; ++x)
        //{
        //    for (int y = 0; y < Dimensions.y; ++y)
        //    {
        //        var pos = new Vector2Int(x, y);
        //        if (!offsetsSet.Contains(pos))
        //            Gizmos.DrawIcon(GetSpace(pos), "CollabError", true);
        //    }
        //}
    }

    private void Start()
    {
        shell = null;
        Shell = PersistantData.main.inventory.EquippedShell;
    }

    public override bool Add(Vector2Int addPos, Program obj)
    {
        var positions = obj.shape.OffsetsShifted(addPos, false);
        var shellPositions = shell.CustArea.OffsetsSet;
        foreach(var pos in positions)
        {
            if(!IsLegal(pos) || !shellPositions.Contains(pos))
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
        foreach (var pos in obj.shape.OffsetsShifted(obj.Pos, false))
        {
            Set(pos, null);
        }
        obj.Hide(this);
        obj.Pos = OutOfBounds;
    }

    public void ResetShell()
    {
        if (shell == null)
            return;

        // Clear UI
        foreach (var program in this)
        {
            program.Hide(this);
        }
        blockedUI.ForEach((e) => RemoveTileUI(e));
        blockedUI.Clear();

        // Reinitialize the grid
        Initialize();

        // Add already installed programs
        foreach (var program in shell.Programs)
        {
            Add(program.location, program.program);
        }

        // Spawn blocked tile UI in blocked tiles
        var offsetsSet = shell.CustArea.OffsetsSet;
        for (int x = 0; x < Dimensions.x; ++x)
        {
            for (int y = 0; y < Dimensions.y; ++y)
            {
                var pos = new Vector2Int(x, y);
                if (!offsetsSet.Contains(pos))
                    blockedUI.Add(SpawnTileUI(pos, TileUI.Type.CustBlocked));
            }
        }
    }
}
