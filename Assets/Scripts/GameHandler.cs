using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    private static GameHandler instance;
    [SerializeField] public Transform resourceTransform = null;
    [SerializeField] public Transform storageTransform = null;

    private List<ResourceNode> resourceNodeList;

    void Awake()
    {
        instance = this;

        resourceNodeList = new List<ResourceNode>();
        resourceNodeList.Add(new ResourceNode(resourceTransform, false));
    }

    private ResourceNode GetResourceNode()
    {
        List<ResourceNode> tempResourceNodes = new List<ResourceNode>(resourceNodeList);
        //Iterate over every node
        for (int i = 0; i < tempResourceNodes.Count; i++)
        {
            //Check if it has resources
            if (!tempResourceNodes[i].HasResources())
            {
                //If no resources, remove from list
                tempResourceNodes.RemoveAt(i);
                i--;
            }
        }

        //If resources are available
        if(tempResourceNodes.Count > 0)
        {
            return resourceNodeList[UnityEngine.Random.Range(0, tempResourceNodes.Count)];
        }
        else
        {
            return null;
        }
    }

    public static ResourceNode GetResourceNode_Static()
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
