using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    private static GameHandler instance;
    [SerializeField] public Transform resourceTransform = null;
    [SerializeField] public Transform storageTransform = null;

    void Awake()
    {
        instance = this;
    }

    private Transform GetResourceNode()
    {
        return resourceTransform;
    }

    public static Transform GetResourceNode_Static()
    {
        return instance.GetResourceNode();
    }

    private Transform GetStorageNode()
    {
        return storageTransform;
    }

    public static Transform GetStorageNode_Static()
    {
        return instance.GetStorageNode();
    }
}
