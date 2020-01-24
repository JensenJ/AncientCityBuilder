using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIUnit : MonoBehaviour, AIInterface
{

    [SerializeField] AIGrid aiGrid = null;
    [SerializeField] public float unitSpeed = 1.0f;
    [SerializeField] Vector3 targetPos;
    [SerializeField] float movementStopDist = 0.5f;

    Action nextTask = delegate { };
    [SerializeField] bool hasCompletedCurrentTask = true;

    List<Vector3> currentPath = null;
    int currentPathIndex = 0;

    void Awake()
    {
        targetPos = transform.position;
    }
    

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
        }
    }

    //Function for moving
    protected void Move()
    {

        bool hasRotated = false;
        //If there is a path
        if(currentPath != null)
        {
            //Get target pos
            Vector3 targetPos = currentPath[currentPathIndex];
            //If within a certain range of next node
            if (Vector3.Distance(transform.position, targetPos) > movementStopDist)
            {
                //Actual transformation
                Vector3 moveDir = (targetPos - transform.position).normalized;

                //Check if rotation is complete
                Quaternion.LookRotation(targetPos, Vector3.up);

                //transform.rotation = Quaternion.Euler(-90, transform.rotation.y, transform.rotation.z);

                transform.position = transform.position + moveDir * unitSpeed * Time.deltaTime;

                //Only move when rotation has finished
                if (hasRotated == true)
                {

                }


            }
            else
            {
                //arrived at destination (each node), increment path index
                currentPathIndex++;
                hasRotated = false;

                //If at final node, stop moving
                if (currentPathIndex >= currentPath.Count)
                {
                    StopMoving();
                    hasCompletedCurrentTask = true;
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
    }

    //Function to stop moving
    public void StopMoving()
    {
        currentPath = null;
    }


    //Sets the target position for movement.
    public void SetTargetPosition(Vector3 target)
    {
        currentPathIndex = 0;
        currentPath = Pathfinding.Instance.FindPath(transform.position, target);

        if(currentPath != null && currentPath.Count > 1)
        {
            currentPath.RemoveAt(0);
        }

        if(currentPath == null)
        {
            Debug.Log("Path not found");
            targetPos = transform.position;
        }
    }

    public void MoveTo(Vector3 position, float stoppingDistance, Action onArrivedAtPosition)
    {
        Debug.Log("Moving to" + position);
        targetPos = position;
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
