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


    [SerializeField] private ResourceNode resourceNode = null;
    [SerializeField] private StorageNode storageNode = null;
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
                resourceNode = GameHandler.GetResourceNode_Static();
                if(resourceNode != null)
                {
                    state = AIState.MovingToResource;
                }
                break;
            //If moving to resource
            case AIState.MovingToResource:
                if (IsIdle())
                {
                    MoveTo(resourceNode.GetPosition(), 0.5f, () =>
                    {
                        state = AIState.Gathering;
                    });
                }
                break;
            //If gathering
            case AIState.Gathering:
                if (IsIdle())
                {
                    if(inventoryAmount >= 5)
                    {
                        storageNode = GameHandler.GetStorageNode_Static();
                        state = AIState.MovingToStorage;
                    }
                    else
                    {
                        resourceNode.GrabResource();
                        inventoryAmount++;
                    }
                }
                break;
            case AIState.MovingToStorage:
                if (IsIdle())
                {
                    MoveTo(storageNode.GetPosition(), 0.5f, () =>
                    {
                        GameResources.AddGoldAmount(inventoryAmount);
                        inventoryAmount = 0;
                        state = AIState.Idle;
                    });
                }
                break;
        }

        Move();
    }
}
