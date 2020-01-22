using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface AIInterface
{
    bool IsIdle();
    void MoveTo(Vector3 position, float stoppingDistance, Action onArrivedAtPosition);

}
