using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGrid : MonoBehaviour
{
    private Pathfinder pathfinding;
    [SerializeField] Vector2Int gridSize = new Vector2Int();

    private void Awake()
    {
        pathfinding = new Pathfinder(gridSize.x, gridSize.y);
    }

    private void Update()
    {

    }

    //Function to get the path based on the current position passed in and the target location
    public List<Vector3> GetPath(Vector3 currentPos, Vector3 targetLocation)
    {
        List<Vector3> path = pathfinding.GetPath(currentPos, targetLocation);

        if(path != null)
        {
            return path;
        }
        else
        {
            return null;
        }

    }
}
