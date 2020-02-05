using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGrid : MonoBehaviour
{
    public static AIGrid Instance { private set; get; }

    public GridSystem<GridNode> pathfindingGrid;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        pathfindingGrid = new GridSystem<GridNode>(30, 30, 1f, Vector3.zero, (GridSystem<GridNode> Grid, int x, int y) => new GridNode(pathfindingGrid, x, y), true, 0.1f);
        pathfindingGrid.GetGridObject(2, 0).SetIsWalkable(false);
    }
}
