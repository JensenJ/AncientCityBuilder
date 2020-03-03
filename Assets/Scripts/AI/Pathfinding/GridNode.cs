using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JUCL.Utilities;

public class GridNode
{
    private JUCLGrid<GridNode> grid;
    public int x;
    public int y;
    private bool isWalkable;

    public GridNode(JUCLGrid<GridNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        isWalkable = true;
    }

    public bool IsWalkabke()
    {
        return isWalkable;
    }
    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
        //grid.TriggerGridObjectChanged(x, y);
    }
}