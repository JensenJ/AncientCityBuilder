using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGatherer : AIUnit
{

    enum AIState 
    {
        Idle,
        MovingToResource,
        Gathering,
        MovingToStorage
    };


    [SerializeField] private Transform resourceTransform = null;
    [SerializeField] private Transform storageTransform = null;
    [SerializeField] private int inventoryAmount = 0;

    private AIState state;

    private void Awake()
    {
        state = AIState.Idle;
    }

    private void Update()
    {
        switch (state) {
            //If idle
            case AIState.Idle:
                resourceTransform = GameHandler.GetResourceNode_Static();
                state = AIState.MovingToResource;
                break;
            //If moving to resource
            case AIState.MovingToResource:
                if (IsIdle())
                {
                    MoveTo(resourceTransform.position, 0.5f, () =>
                    {
                        state = AIState.Gathering;
                    });
                }
                break;
            //If gathering
            case AIState.Gathering:
                if (IsIdle())
                {
                    if(inventoryAmount > 0)
                    {
                        storageTransform = GameHandler.GetStorageNode_Static();
                        state = AIState.MovingToStorage;
                    }
                    else
                    {
                        inventoryAmount++;
                    }
                }
                break;
            case AIState.MovingToStorage:
                if (IsIdle())
                {
                    MoveTo(storageTransform.position, 0.5f, () =>
                    {
                        inventoryAmount = 0;
                        state = AIState.Idle;
                    });
                }
                break;
        }

        Move();
    }
}
