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
        pathfindingGrid = new GridSystem<GridNode>(30, 15, 1f, Vector3.zero, (GridSystem<GridNode> Grid, int x, int y) => new GridNode(pathfindingGrid, x, y), true);
        pathfindingGrid.GetGridObject(2, 0).SetIsWalkable(false);
    }

    private void Update()
    {

    }

    //Function to get the path based on the current position passed in and the target location
    public List<Vector3> GetPath(Vector3 currentPos, Vector3 targetLocation)
    {
        //List<Vector3> path = pathfinding.GetPath(currentPos, targetLocation);

        //if(path != null)
        //{
        //    return path;
        //}
        //else
        //{
        //    return null;
        //}

        return null;
    }
}
