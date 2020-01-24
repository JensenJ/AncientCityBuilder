using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageNode
{
    private Transform resourceNodeTransform;

    public StorageNode(Transform resourceNodeTransform)
    {
        this.resourceNodeTransform = resourceNodeTransform;
    }

    public Vector3 GetPosition()
    {
        return resourceNodeTransform.position;
    }
}
