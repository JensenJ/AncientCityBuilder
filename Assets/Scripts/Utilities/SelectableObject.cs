using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    public static event EventHandler OnObjectSelected;
    private static Transform selectedObject;

    void OnMouseDown()
    {
        selectedObject = transform;
        OnObjectSelected?.Invoke(null, EventArgs.Empty);
    }

    public static Transform GetSelectedObject()
    {
        return selectedObject;
    }
}
