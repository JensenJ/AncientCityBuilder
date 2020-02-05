using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;

[RequireComponent(typeof(SelectableObject))]
public class AIUnit : MonoBehaviour
{
    [SerializeField] private ConvertedEntityHolder convertedEntityHolder;

    [SerializeField] AIGrid aiGrid = null;
    [SerializeField] public float unitSpeed = 1.0f;
    [SerializeField] float movementStopDist = 0.1f;
    [SerializeField] float gridCellSize = 1.0f;

    Action nextTask = delegate { };
    [SerializeField] bool hasCompletedCurrentTask = true;

    List<Vector3> currentPath = null;
    int currentPathIndex = 0;

    void Awake()
    {
        CreateEvents();
    }
    
    protected void CreateEvents()
    {
        SelectableObject.OnObjectSelected += delegate (object sender, EventArgs e)
        {
            SelectUnit();
        };

    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(convertedEntityHolder.GetEntity());
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
        }
    }

    public void SelectUnit()
    {
        RTSCameraController.instance.followTransform = SelectableObject.GetSelectedObject();
    }

    //Function for moving
    protected void Move()
    {
        //Entity data getting
        Entity entity = convertedEntityHolder.GetEntity();
        EntityManager entityManager = convertedEntityHolder.GetEntityManager();

        PathFollowComponent pathFollow = entityManager.GetComponentData<PathFollowComponent>(entity);
        DynamicBuffer<PathPosition> pathPositionBuffer = entityManager.GetBuffer<PathPosition>(entity);

        Debug.Log(pathFollow.pathIndex);
        if(pathFollow.pathIndex >= 0)
        {
            PathPosition pathPosition = pathPositionBuffer[pathFollow.pathIndex];

            float3 targetPosition = new float3(pathPosition.position.x, 0, pathPosition.position.y) + new float3(gridCellSize, 0, gridCellSize) * 0.5f;
            float3 moveDir = math.normalizesafe(targetPosition - (float3)transform.position);

            transform.position += (Vector3)(moveDir * 10f * Time.deltaTime);
            if (math.distance(transform.position, targetPosition) < 0.1f)
            {
                if(pathFollow.pathIndex == 0)
                {
                    hasCompletedCurrentTask = true;
                }
                //Debug.Log(pathPosition.position.x + "   " + pathPosition.position.y);
                pathFollow.pathIndex--;
                entityManager.SetComponentData(entity, pathFollow);

            }
        }

        if (hasCompletedCurrentTask == true)
        {
            if (nextTask != null)
            {
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

    public void MoveTo(Vector3 position, float stoppingDistance, Action onArrivedAtPosition)
    {
        Debug.Log("Moving to" + position);

        SetTargetPosition(position);
        movementStopDist = stoppingDistance;
        hasCompletedCurrentTask = false;
        nextTask = onArrivedAtPosition;
    }

    public bool IsIdle()
    {
        return hasCompletedCurrentTask;
    }
}
