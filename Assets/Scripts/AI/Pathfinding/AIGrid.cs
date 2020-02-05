using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGrid : MonoBehaviour
{
    public static AIGrid Instance { private set; get; }

    public GridSystem<GridNode> pathfindingGrid;

    //Variables for grid setup
    [SerializeField] Vector2Int gridSize = new Vector2Int(30, 30);
    [SerializeField] float cellSize = 1.0f;
    [SerializeField] bool debugMode = false;
    [SerializeField] float debugTextHeight = 0.1f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //Create pathfinding grid
        pathfindingGrid = new GridSystem<GridNode>(gridSize.x, gridSize.y, cellSize, Vector3.zero, (GridSystem<GridNode> Grid, int x, int y) => new GridNode(pathfindingGrid, x, y), debugMode, debugTextHeight);
        pathfindingGrid.GetGridObject(2, 0).SetIsWalkable(false);
    }
}
