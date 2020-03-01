using System;
using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;

public class AIUnit : MonoBehaviour
{
    //Entity reference
    [SerializeField] private ConvertedEntityHolder convertedEntityHolder = null;

    [SerializeField] AIGrid aiGrid = null;
    [SerializeField] public float unitSpeed = 1.0f;
    [SerializeField] float movementStopDist = 0.1f;

    Action nextTask = delegate { };
    [SerializeField] bool hasCompletedCurrentTask = true;

    float gridCellSize = 1.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        //If hasnt been assigned
        if(aiGrid == null)
        {
            // Try find it
            aiGrid = GameObject.FindGameObjectWithTag("AIGrid").GetComponent<AIGrid>();
            //If still null
            if(aiGrid == null)
            {
                //Log error
                Debug.LogError("AI grid has not been assigned or could not be found, there can only be one grid in a scene.");
            }
            else
            {
                gridCellSize = AIGrid.Instance.pathfindingGrid.GetCellSize();
            }
        }
    }

    //Function for moving
    protected void Move()
    {
        //Entity data getting
        Entity entity = convertedEntityHolder.GetEntity();
        EntityManager entityManager = convertedEntityHolder.GetEntityManager();

        if(entityManager == null)
        {
            return;
        }
        PathFollowComponent pathFollow = entityManager.GetComponentData<PathFollowComponent>(entity);
        DynamicBuffer<PathPosition> pathPositionBuffer = entityManager.GetBuffer<PathPosition>(entity);

        //If still following path
        if(pathFollow.pathIndex >= 0)
        {
            //Get path position
            PathPosition pathPosition = pathPositionBuffer[pathFollow.pathIndex];

            //Get world position for each node
            float3 targetPosition = new float3(pathPosition.position.x, 0, pathPosition.position.y) + Vector3.one * new float3(gridCellSize, 0, gridCellSize) * 0.5f;
            //Calculate move direction
            float3 moveDir = math.normalizesafe(targetPosition - (float3)transform.position);

            //Move the ai to the next node
            transform.position += (Vector3)(moveDir * (3f + unitSpeed) * Time.deltaTime);
            //Check if within radius of next node
            if (math.distance(transform.position, targetPosition) < movementStopDist)
            {
                //If reached destination
                if(pathFollow.pathIndex == 0)
                {
                    hasCompletedCurrentTask = true;
                }
                //Next node
                pathFollow.pathIndex--;
                //Update pathfollow array
                entityManager.SetComponentData(entity, pathFollow);

            }
        }

        //If current task completed
        if (hasCompletedCurrentTask == true)
        {
            //Null pointer check for next task
            if (nextTask != null)
            {
                //Invoke next task
                nextTask.Invoke();
                nextTask = null;
            }
        }
    }

    //Function to set new target position
    private void SetTargetPosition(Vector3 position)
    {
        //Entity data getting
        Entity entity = convertedEntityHolder.GetEntity();
        EntityManager entityManager = convertedEntityHolder.GetEntityManager();

        //Setting new path data
        entityManager.AddComponentData(entity, new PathfindingComponentData
        {
            startPosition = new int2((int)transform.position.x, (int)transform.position.z),
            endPosition = new int2((int)position.x, (int)position.z)
        });

    }

    //Function to move to a certain position, and then set the next task
    public void MoveTo(Vector3 position, float stoppingDistance, Action onArrivedAtPosition)
    {
        Debug.Log("Moving to" + position);
        SetTargetPosition(position);
        movementStopDist = stoppingDistance;
        hasCompletedCurrentTask = false;
        nextTask = onArrivedAtPosition;
    }

    //Getter for whether the ai has finished its current task
    public bool IsIdle()
    {
        return hasCompletedCurrentTask;
    }
}
