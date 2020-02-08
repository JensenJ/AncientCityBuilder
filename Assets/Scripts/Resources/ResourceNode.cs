using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceNode
{
    private Transform resourceNodeTransform;
    private bool isInfinite;

    private int resourceAmount;

    public ResourceNode(Transform resourceNodeTransform, bool isInfinite)
    {
        this.resourceNodeTransform = resourceNodeTransform;
        this.isInfinite = isInfinite;
        if (isInfinite)
        {
            resourceAmount = int.MaxValue;
        }
        else
        {
            resourceAmount = 100;
        }
    }

    public Vector3 GetPosition()
    {
        return resourceNodeTransform.position;
    }

    public void GrabResource()
    {
        if (!isInfinite)
        {
            resourceAmount -= 1;
            if (resourceAmount <= 0)
            {
                resourceNodeTransform.GetComponent<MeshRenderer>().enabled = false;
                resourceNodeTransform.GetComponent<BoxCollider>().enabled = false;
            } 
        }
    }

    public bool HasResources()
    {
        return resourceAmount > 0;
    }
}
