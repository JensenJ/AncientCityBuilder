using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGatherer : AIUnit
{

    [SerializeField] private Transform resourceTransform = null;
    [SerializeField] private Transform storageTransform = null;

    private void Start()
    {
        MoveTo(resourceTransform.position, 1.0f, () =>
        {
            MoveTo(storageTransform.position, 1.0f, null);
        });
    }
}
