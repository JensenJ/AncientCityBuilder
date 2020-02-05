using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode
{
    private GridSystem<GridNode> grid;
    public int x;
    public int y;
    private bool isWalkable;

    public GridNode(GridSystem<GridNode> grid, int x, int y)
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