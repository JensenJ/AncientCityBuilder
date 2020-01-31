using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pathfinder))]
public class AIGrid : MonoBehaviour
{
    private Pathfinder pathfinding;
    [SerializeField] LayerMask[] unwalkableMasks = null;
    [SerializeField] Vector2Int gridSize = new Vector2Int();
    [SerializeField] Vector3 gridOriginPosition= new Vector3();
    [SerializeField] float walkableCheckRadius = 0.0f;
    [SerializeField] bool showDebug = false;

    private void Awake()
    {
        gridOriginPosition += transform.position;
        //pathfinding = new Pathfinding(gridSize.x, gridSize.y, gridOriginPosition, unwalkableMasks, walkableCheckRadius, showDebug);
        pathfinding = GetComponent<Pathfinder>();
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
